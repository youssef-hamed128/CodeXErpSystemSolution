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
                invoice.Status = model.Status;
                invoice.PaidAmount = model.PaidAmount;
                invoice.CreatedBy = userId;

                invoice.SubTotal = model.Items.Sum(i => i.Quantity * i.UnitPrice);
                if (model.DiscountPercentage > 0) 
                {
                    invoice.DiscountPercentage = model.DiscountPercentage;
                    invoice.DiscountAmount = invoice.SubTotal * (model.DiscountPercentage / 100m);
                }
                else if (model.DiscountAmount > 0 && invoice.SubTotal > 0)
                {
                    invoice.DiscountAmount = model.DiscountAmount;
                    invoice.DiscountPercentage = Math.Round((model.DiscountAmount / invoice.SubTotal) * 100m, 2);
                }
                decimal taxRate = model.TaxPercentage / 100m;
                foreach (var item in invoice.Items)
                {
                    item.TaxAmount = (item.Quantity * item.UnitPrice) * taxRate;
                    item.Total = (item.Quantity * item.UnitPrice) + item.TaxAmount;
                }
                invoice.TaxAmount = invoice.Items.Sum(i => i.TaxAmount);
                invoice.TotalAmount = (invoice.SubTotal - invoice.DiscountAmount) + invoice.TaxAmount;
                invoice.PaymentMethod = model.PaymentMethod;

                if (model.PaymentMethod == PaymentMethod.BalanceDeduction)
                {
                    decimal deductAmt = (model.PaidAmount > 0) ? model.PaidAmount : invoice.TotalAmount;
                    if (deductAmt > invoice.TotalAmount) deductAmt = invoice.TotalAmount;
                    invoice.PaidAmount = deductAmt;
                    invoice.Status = (deductAmt >= invoice.TotalAmount - 0.01m) ? InvoiceStatus.Paid : (deductAmt > 0 ? InvoiceStatus.Partial : InvoiceStatus.Unpaid);
                }
                else if (invoice.Status == InvoiceStatus.Paid || (invoice.PaidAmount >= invoice.TotalAmount && invoice.TotalAmount > 0))
                {
                    invoice.Status = InvoiceStatus.Paid;
                    invoice.PaidAmount = invoice.TotalAmount;
                }
                else if (invoice.Status == InvoiceStatus.Partial || (invoice.PaidAmount > 0 && invoice.PaidAmount < invoice.TotalAmount))
                {
                    invoice.Status = InvoiceStatus.Partial;
                    if (invoice.PaidAmount < 0) invoice.PaidAmount = 0;
                }
                else
                {
                    invoice.Status = InvoiceStatus.Unpaid;
                    invoice.PaidAmount = 0;
                }
                unitOfWork.GetRepository<Invoice>().Add(invoice);

                //--------------------- تحديث المخزون
                foreach (var item in invoice.Items)
                {
                    var stock = await unitOfWork.GetRepository<StockQuantity>().FirstOrDefaultAsync(sq => sq.ProductId == item.ProductId && sq.WarehouseId == model.WarehouseId, true, ct);
                    bool isStockAddition = model.Type == InvoiceType.Purchase || model.Type == InvoiceType.SalesReturn;
                    if (stock == null)
                    {
                        if (!isStockAddition) throw new InvalidOperationException("تنبيه: الصنف غير موجود في المخزون (الرصيد: 0)");
                        stock = new StockQuantity { ProductId = item.ProductId, WarehouseId = model.WarehouseId, Quantity = item.Quantity, CreatedBy = userId };
                        unitOfWork.GetRepository<StockQuantity>().Add(stock);
                    }
                    else
                    {
                        if (isStockAddition)
                            stock.Quantity += item.Quantity;
                        else
                        {
                            if (stock.Quantity <= 0)
                                throw new InvalidOperationException("تنبيه: الصنف غير موجود في المخزون (الرصيد: 0)");
                            if (stock.Quantity < item.Quantity)
                                throw new InvalidOperationException($"الكمية المدخلة أكبر من الكمية المتوفرة في المخزون (المتوفر: {stock.Quantity})");
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

                    if (model.Type == InvoiceType.Purchase)
                    {
                        var product = await unitOfWork.GetRepository<Product>().GetById(item.ProductId, ct);
                        if (product != null)
                        {
                            product.PurchasePrice = item.UnitPrice;
                            var modelItem = model.Items.FirstOrDefault(m => m.ProductId == item.ProductId);
                            if (modelItem != null && modelItem.SalePrice.HasValue && modelItem.SalePrice.Value >= 0)
                            {
                                product.SalePrice = modelItem.SalePrice.Value;
                            }
                            unitOfWork.GetRepository<Product>().Update(product);
                        }
                    }
                }
                //--------------------------تحديث الارصده للعملاء و الموردين 
                if (model.CustomerId.HasValue)
                {
                    var customer = await unitOfWork.GetRepository<Customer>().GetById(model.CustomerId.Value,ct);
                    if (customer != null)
                    {
                        if (model.PaymentMethod == PaymentMethod.BalanceDeduction)
                        {
                            if (model.Type == InvoiceType.Sales) customer.Balance = (customer.Balance ?? 0) + invoice.TotalAmount;
                            else if (model.Type == InvoiceType.SalesReturn) customer.Balance = (customer.Balance ?? 0) - invoice.TotalAmount;
                        }
                        else
                        {
                            decimal remAmount = invoice.TotalAmount - invoice.PaidAmount;
                            if (model.Type == InvoiceType.Sales) customer.Balance = (customer.Balance ?? 0) + remAmount;
                            else if (model.Type == InvoiceType.SalesReturn) customer.Balance = (customer.Balance ?? 0) - remAmount;
                        }
                        unitOfWork.GetRepository<Customer>().Update(customer);
                    }
                }
                if (model.SupplierId.HasValue)
                {
                    var supplier = await unitOfWork.GetRepository<Supplier>().GetById(model.SupplierId.Value, ct);
                    if (supplier != null)
                    {
                        if (model.PaymentMethod == PaymentMethod.BalanceDeduction)
                        {
                            if (model.Type == InvoiceType.Purchase) supplier.Balance = (supplier.Balance ?? 0) + invoice.TotalAmount;
                            else if (model.Type == InvoiceType.PurchaseReturn) supplier.Balance = (supplier.Balance ?? 0) - invoice.TotalAmount;
                        }
                        else
                        {
                            decimal remAmount = invoice.TotalAmount - invoice.PaidAmount;
                            if (model.Type == InvoiceType.Purchase) supplier.Balance = (supplier.Balance ?? 0) + remAmount;
                            else if (model.Type == InvoiceType.PurchaseReturn) supplier.Balance = (supplier.Balance ?? 0) - remAmount;
                        }
                        unitOfWork.GetRepository<Supplier>().Update(supplier);
                    }
                }

                await unitOfWork.CompleteAsync(ct);

                // إنشاء سند في جدول السندات فقط في حالة السداد الجزئي (مدفوعة جزئياً) بعد حفظ الفاتورة لضمان وجود InvoiceId
                if (invoice.PaidAmount > 0.01m && invoice.PaidAmount < invoice.TotalAmount - 0.01m && model.PaymentMethod != PaymentMethod.BalanceDeduction)
                {
                    var allPayments = await unitOfWork.GetRepository<Payment>().GetAll(false, ct);
                    var maxNumber = allPayments.Any() ? allPayments.Max(p => int.TryParse(p.ReceiptNumber, out int n) ? n : 0) : 1000;
                    string nextReceiptNum = (maxNumber + 1).ToString();

                    var partialPaymentReceipt = new Payment
                    {
                        ReceiptNumber = nextReceiptNum,
                        Date = DateTime.Now,
                        Amount = invoice.PaidAmount,
                        PaymentMethod = CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Cash,
                        Reference = $"سداد جزئي عند إنشاء فاتورة رقم {invoice.InvoiceNumber}",
                        InvoiceId = invoice.Id,
                        CustomerId = invoice.CustomerId,
                        SupplierId = invoice.SupplierId,
                        CreatedBy = userId
                    };
                    unitOfWork.GetRepository<Payment>().Add(partialPaymentReceipt);
                    await unitOfWork.CompleteAsync(ct);
                }

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
