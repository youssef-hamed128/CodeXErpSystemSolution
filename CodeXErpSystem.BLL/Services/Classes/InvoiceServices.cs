using AutoMapper;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Invoice;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Entites.Enums;
using CodeXErpSystem.DAL.Repository.Inetrfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class InvoiceServices : IInvoiceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public InvoiceServices(IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<string> GenerateInvoiceNumberAsync(InvoiceType type)
        {
            var prefix = type switch
            {
                InvoiceType.Sales => "INV",
                InvoiceType.Purchase => "PUR",
                InvoiceType.SalesReturn => "SRT",
                InvoiceType.PurchaseReturn => "PRT",

                _ => "DOC"
            };
            var lastInvoice = await unitOfWork.GetRepository<Invoice>().FindAsync(i => i.Type == type, null, i => i.OrderByDescending(x => x.Id), false);
            var last = lastInvoice.FirstOrDefault();

            if (last == null || string.IsNullOrEmpty(last.InvoiceNumber))
                return $"{prefix}-{DateTime.Now.Year}-00001";

            int lastDashIndex = last.InvoiceNumber.LastIndexOf('-');
            if (lastDashIndex >= 0 && lastDashIndex < last.InvoiceNumber.Length - 1)
            {
                string numStr = last.InvoiceNumber.Substring(lastDashIndex + 1);
                if (int.TryParse(numStr, out int lastNum))
                {
                    return $"{prefix}-{DateTime.Now.Year}-{(lastNum + 1):D5}";
                }
            }

            return $"{prefix}-{DateTime.Now.Year}-{new Random().Next(10000,99999)}";
        }
        public async Task<InvoiceViewModel> CreateInvoiceAsync(InvoiceCreateViewModel model,string userId, CancellationToken ct = default)
        {
            await unitOfWork.BeginTransactionAsync(ct);
            try
            {//--------------------- حساب صافي السعر  و حسباب الخصم و حساب الضريبه و حساب الاجمالي
                var invoice = mapper.Map<Invoice>(model);
                invoice.ReferenceNumber = model.ReferenceNumber;
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync(model.Type);
                invoice.Status = InvoiceStatus.Paid;
                invoice.CreatedBy = userId;

                invoice.SubTotal = model.Items.Sum(i => i.Quantity * i.UnitPrice);
                if(model.DiscountPercentage > 0) invoice.DiscountAmount = invoice.SubTotal * (model.DiscountPercentage / 100);
                decimal taxRate = model.TaxPercentage / 100m;
                foreach (var item in invoice.Items)
                {
                    item.TaxAmount = (item.Quantity * item.UnitPrice) * taxRate;
                    item.Total = (item.Quantity * item.UnitPrice) + item.TaxAmount;
                }
                invoice.TaxAmount = invoice.Items.Sum(i => i.TaxAmount);
                invoice.TotalAmount = (invoice.SubTotal - invoice.DiscountAmount) + invoice.TaxAmount;
                unitOfWork.GetRepository<Invoice>().Add(invoice);
                //--------------------- تحديث المخزون
                foreach (var item in invoice.Items)
                {
                    var stock = await unitOfWork.GetRepository<StockQuantity>().FirstOrDefaultAsync(sq => sq.ProductId == item.ProductId && sq.WarehouseId == model.WarehouseId, true, ct);
                    bool isStockAddition = model.Type == InvoiceType.Purchase || model.Type == InvoiceType.SalesReturn;
                    if (stock == null)
                    {
                        if (!isStockAddition) throw new Exception("الصنف غير موجود المخزن");
                        stock = new StockQuantity { ProductId = item.ProductId, WarehouseId = model.WarehouseId, Quantity = item.Quantity, CreatedBy = userId };
                        unitOfWork.GetRepository<StockQuantity>().Add(stock);
                    }
                    else
                    {
                        if (isStockAddition)
                            stock.Quantity += item.Quantity;
                        else
                        {
                            if (stock.Quantity < item.Quantity)
                                throw new Exception("رصيد المخزن لا يكفي");
                            stock.Quantity -= item.Quantity;
                        }
                        unitOfWork.GetRepository<StockQuantity>().Update(stock);
                    }
                    unitOfWork.GetRepository<StockTransaction>().Add(new StockTransaction()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Type = isStockAddition ? StockTransactionType.In : StockTransactionType.Out,
                        SourceWarehouseId = !isStockAddition ? model.WarehouseId : null,
                        DestWarehouseId = isStockAddition ? model.WarehouseId : null,
                        ReferenceId = invoice.InvoiceNumber,
                        CreatedBy = userId
                    });
                }
                //--------------------------تحديث الارصده للعملاء و الموردين 
                if (model.CustomerId.HasValue)
                {
                    var customer = await unitOfWork.GetRepository<Customer>().GetById(model.CustomerId.Value,ct);
                    if (customer != null)
                    {
                        if (model.Type == InvoiceType.Sales) customer.Balance += invoice.TotalAmount;
                        else if (model.Type == InvoiceType.SalesReturn) customer.Balance -= invoice.TotalAmount;
                        unitOfWork.GetRepository<Customer>().Update(customer);
                    }
                }
                if (model.SupplierId.HasValue)
                {
                    var supplier = await unitOfWork.GetRepository<Supplier>().GetById(model.SupplierId.Value, ct);
                    if (supplier != null)
                    {
                        if(model.Type == InvoiceType.Purchase) supplier.Balance += invoice.TotalAmount;
                        else if (model.Type == InvoiceType.PurchaseReturn) supplier.Balance -= invoice.TotalAmount;
                        unitOfWork.GetRepository<Supplier>().Update(supplier);
                    }
                }
                await unitOfWork.CompleteAsync(ct);
                await unitOfWork.CommitTransactionAsync(ct);
                return mapper.Map<InvoiceViewModel>(invoice);
                
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(ct); 
                throw;
            }
        }

        public async Task<InvoiceViewModel?> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken ct = default)
        {
            var invoices = await unitOfWork.GetRepository<Invoice>().FindAsync(
                filter: i => i.InvoiceNumber == invoiceNumber,
                includeProperties: "Items,Items.Product,Customer,Supplier",
                orderBy: null,
                isTracked: false,
                ct: ct
            );

            var invoice = invoices.FirstOrDefault();
            if (invoice == null) return null;

            return mapper.Map<InvoiceViewModel>(invoice);
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus newStatus, CancellationToken ct = default)
        {
            var invoice = await unitOfWork.GetRepository<Invoice>().GetById(invoiceId, ct);
            if (invoice == null) return false;

            invoice.Status = newStatus;
            unitOfWork.GetRepository<Invoice>().Update(invoice);
            return await unitOfWork.CompleteAsync() > 0;
        }


    }
}
