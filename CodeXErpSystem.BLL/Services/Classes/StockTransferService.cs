using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Products;
using CodeXErpSystem.BLL.ViewModels.Warehouses;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class StockTransferService : IStockTransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockTransferService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public StockTransferViewModel GetStockTransferInitialData()
        {
            var warehouses = _unitOfWork.GetRepository<Warehouse>().GetAll(false).Result;
            var products = _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "Category,StockQuantities").Result;
            var categories = _unitOfWork.GetRepository<ProductCategory>().GetAll(false).Result;

            var productsVm = _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel>>(products).ToList();
            foreach (var p in productsVm)
            {
                var entity = products.FirstOrDefault(x => x.Id == p.Id);
                if (entity != null && entity.StockQuantities != null)
                {
                    // If a product is in the same warehouse multiple times, GroupBy handles it, though shouldn't happen.
                    p.StockByWarehouse = entity.StockQuantities
                        .GroupBy(sq => sq.WarehouseId)
                        .ToDictionary(g => g.Key, g => g.Sum(sq => sq.Quantity));
                        
                    p.AvailableQuantity = entity.StockQuantities.Sum(sq => sq.Quantity);
                }
            }

            return new StockTransferViewModel
            {
                Warehouses = _mapper.Map<IEnumerable<WarehouseViewModel>>(warehouses).ToList(),
                Products = productsVm,
                Categories = _mapper.Map<IEnumerable<ProductCategoryViewModel>>(categories).ToList(),
                TransferItems = new List<StockTransferItemViewModel>()
            };
        }

        public async Task CreateAsync(StockTransferViewModel model)
        {
            if (model == null || model.TransferItems == null || !model.TransferItems.Any())
            {
                throw new Exception("يجب إضافة منتج واحد على الأقل لعملية التحويل.");
            }

            int srcWhId = model.SourceWarehouseId > 0 ? model.SourceWarehouseId : model.FromWarehouseId;
            int destWhId = model.DestWarehouseId > 0 ? model.DestWarehouseId : model.ToWarehouseId;

            if (srcWhId <= 0 || destWhId <= 0)
            {
                throw new Exception("الرجاء تحديد مستودع المصدر ومستودع الوجهة.");
            }

            if (srcWhId == destWhId)
            {
                throw new Exception("لا يمكن تحويل المخزون لنفس المستودع! اختر مستودع وجهة مختلف.");
            }

            var stockRepo = _unitOfWork.GetRepository<StockQuantity>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var transRepo = _unitOfWork.GetRepository<StockTransaction>();

            var allStock = await stockRepo.GetAll(false);
            var allProducts = await productRepo.GetAll(false);

            string refNumber = string.IsNullOrWhiteSpace(model.ReferenceNumber) ? $"TRF-{DateTime.Now:yyyyMMdd-HHmmss}" : model.ReferenceNumber;
            DateTime transferDate = model.TransferDate == default ? DateTime.Now : model.TransferDate;

            foreach (var item in model.TransferItems)
            {
                if (item.TransferredQuantity <= 0) continue;

                var prod = allProducts.FirstOrDefault(p => p.Id == item.ProductId);
                string prodName = prod?.Name ?? $"رقم {item.ProductId}";

                var srcStock = allStock.FirstOrDefault(sq => sq.ProductId == item.ProductId && sq.WarehouseId == srcWhId);
                if (srcStock == null || srcStock.Quantity < item.TransferredQuantity)
                {
                    decimal avail = srcStock?.Quantity ?? 0;
                    throw new Exception($"الكمية المتاحة في مستودع المصدر غير كافية للمنتج ({prodName}). المتاح: {avail} المطلوب: {item.TransferredQuantity}");
                }

                // Deduct from Source Warehouse
                srcStock.Quantity -= item.TransferredQuantity;
                stockRepo.Update(srcStock);

                // Add to Destination Warehouse
                var destStock = allStock.FirstOrDefault(sq => sq.ProductId == item.ProductId && sq.WarehouseId == destWhId);
                if (destStock != null)
                {
                    destStock.Quantity += item.TransferredQuantity;
                    stockRepo.Update(destStock);
                }
                else
                {
                    var newStock = new StockQuantity
                    {
                        ProductId = item.ProductId,
                        WarehouseId = destWhId,
                        Quantity = item.TransferredQuantity,
                        CreatedBy = "System",
                        CreatedAt = DateTime.Now
                    };
                    stockRepo.Add(newStock);
                }

                // Record StockTransaction
                var transaction = new StockTransaction
                {
                    ProductId = item.ProductId,
                    SourceWarehouseId = srcWhId,
                    DestWarehouseId = destWhId,
                    Quantity = item.TransferredQuantity,
                    Type = CodeXErpSystem.DAL.Entites.Enums.StockTransactionType.Transfer,
                    Date = transferDate,
                    ReferenceId = refNumber,
                    Note = string.IsNullOrWhiteSpace(model.Notes) ? $"تحويل مخزني من المستودع {srcWhId} إلى المستودع {destWhId}" : model.Notes,
                    CreatedBy = "System",
                    CreatedAt = DateTime.Now
                };
                transRepo.Add(transaction);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(StockTransferViewModel model)
        {
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            await Task.CompletedTask;
        }
    }
}
