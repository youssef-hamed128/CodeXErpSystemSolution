using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Products;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<ProductViewModel> GetAllProducts()
        {
            var entities = _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "Category,StockQuantities,StockQuantities.Warehouse").Result;
            var viewModels = _mapper.Map<IEnumerable<ProductViewModel>>(entities).ToList();
            
            // Map the warehouse names manually
            foreach (var vm in viewModels)
            {
                var entity = entities.FirstOrDefault(e => e.Id == vm.Id);
                if (entity != null && entity.StockQuantities != null && entity.StockQuantities.Any())
                {
                    var activeStocks = entity.StockQuantities.Where(sq => sq.Quantity > 0).ToList();
                    if (activeStocks.Any())
                    {
                        var whNames = activeStocks.Select(sq => sq.Warehouse?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList();
                        vm.WarehouseName = string.Join(", ", whNames);
                    }
                    else
                    {
                        var whNames = entity.StockQuantities.Select(sq => sq.Warehouse?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList();
                        vm.WarehouseName = whNames.Any() ? string.Join(", ", whNames) : "المخزن الرئيسي";
                    }
                    vm.AvailableQuantity = entity.StockQuantities.Sum(sq => sq.Quantity);
                }
                else
                {
                    vm.WarehouseName = "المخزن الرئيسي";
                    vm.AvailableQuantity = 0;
                }

                if (vm.AvailableQuantity == 0) vm.Status = "نفذت الكمية";
                else if (vm.AvailableQuantity <= vm.MinQuantity) vm.Status = "منخفض";
                else vm.Status = "متاح";
            }
            return viewModels;
        }

        public async Task CreateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            var existingCode = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Code == model.Code);
            if (existingCode.Any())
                throw new System.InvalidOperationException("رقم الكود مكرر. يرجى إدخال كود مختلف.");

            var existingName = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Name == model.Name);
            if (existingName.Any())
                throw new System.InvalidOperationException("اسم المنتج مكرر. يرجى إدخال اسم مختلف.");

            var entity = _mapper.Map<Product>(model);
            _unitOfWork.GetRepository<Product>().Add(entity);
            await _unitOfWork.CompleteAsync();

            Warehouse? mainWarehouse = null;
            if (model.WarehouseId.HasValue && model.WarehouseId.Value > 0)
            {
                mainWarehouse = (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => w.Id == model.WarehouseId.Value && !w.IsDeleted)).FirstOrDefault();
            }
            if (mainWarehouse == null)
            {
                mainWarehouse = (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => w.Name == "المخزن الرئيسي" && !w.IsDeleted)).FirstOrDefault()
                                ?? (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => !w.IsDeleted)).FirstOrDefault();
            }
            if (mainWarehouse != null && model.InitialQuantity > 0)
            {
                var sq = new StockQuantity
                {
                    ProductId = entity.Id,
                    WarehouseId = mainWarehouse.Id,
                    Quantity = model.InitialQuantity
                };
                _unitOfWork.GetRepository<StockQuantity>().Add(sq);

                var st = new StockTransaction
                {
                    ProductId = entity.Id,
                    DestWarehouseId = mainWarehouse.Id,
                    Quantity = model.InitialQuantity,
                    Type = DAL.Entites.Enums.StockTransactionType.In,
                    Note = "رصيد افتتاحي عند إضافة المنتج",
                    Date = System.DateTime.UtcNow
                };
                _unitOfWork.GetRepository<StockTransaction>().Add(st);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task UpdateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            var existingCode = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Code == model.Code && p.Id != model.Id);
            if (existingCode.Any())
                throw new System.InvalidOperationException("رقم الكود مكرر لمنتج آخر. يرجى إدخال كود مختلف.");

            var existingName = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Name == model.Name && p.Id != model.Id);
            if (existingName.Any())
                throw new System.InvalidOperationException("اسم المنتج مكرر لمنتج آخر. يرجى إدخال اسم مختلف.");

            var entity = _mapper.Map<Product>(model);
            _unitOfWork.GetRepository<Product>().Update(entity);
            await _unitOfWork.CompleteAsync();

            if (model.InitialQuantity >= 0)
            {
                Warehouse? mainWarehouse = null;
                if (model.WarehouseId.HasValue && model.WarehouseId.Value > 0)
                {
                    mainWarehouse = (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => w.Id == model.WarehouseId.Value && !w.IsDeleted)).FirstOrDefault();
                }
                if (mainWarehouse == null)
                {
                    mainWarehouse = (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => w.Name == "المخزن الرئيسي" && !w.IsDeleted)).FirstOrDefault()
                                        ?? (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => !w.IsDeleted)).FirstOrDefault();
                }
                if (mainWarehouse != null)
                {
                    var sqList = await _unitOfWork.GetRepository<StockQuantity>().FindAsync(s => s.ProductId == model.Id && s.WarehouseId == mainWarehouse.Id);
                    var sq = sqList.FirstOrDefault();
                    if (sq != null)
                    {
                        if (sq.Quantity != model.InitialQuantity)
                        {
                            decimal diff = model.InitialQuantity - sq.Quantity;
                            sq.Quantity = model.InitialQuantity;
                            _unitOfWork.GetRepository<StockQuantity>().Update(sq);

                            var st = new StockTransaction
                            {
                                ProductId = model.Id,
                                DestWarehouseId = diff >= 0 ? mainWarehouse.Id : null,
                                SourceWarehouseId = diff < 0 ? mainWarehouse.Id : null,
                                Quantity = System.Math.Abs(diff),
                                Type = diff >= 0 ? DAL.Entites.Enums.StockTransactionType.In : DAL.Entites.Enums.StockTransactionType.Out,
                                Note = "تعديل الرصيد الافتتاحي للمنتج",
                                Date = System.DateTime.UtcNow
                            };
                            _unitOfWork.GetRepository<StockTransaction>().Add(st);
                            await _unitOfWork.CompleteAsync();
                        }
                    }
                    else if (model.InitialQuantity > 0)
                    {
                        sq = new StockQuantity
                        {
                            ProductId = model.Id,
                            WarehouseId = mainWarehouse.Id,
                            Quantity = model.InitialQuantity
                        };
                        _unitOfWork.GetRepository<StockQuantity>().Add(sq);

                        var st = new StockTransaction
                        {
                            ProductId = model.Id,
                            DestWarehouseId = mainWarehouse.Id,
                            Quantity = model.InitialQuantity,
                            Type = DAL.Entites.Enums.StockTransactionType.In,
                            Note = "رصيد افتتاحي للمنتج",
                            Date = System.DateTime.UtcNow
                        };
                        _unitOfWork.GetRepository<StockTransaction>().Add(st);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Product>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}


