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
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockAdjustmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public StockAdjustmentViewModel GetStockAdjustmentInitialData()
        {
            var warehouses = _unitOfWork.GetRepository<Warehouse>().GetAll(false).Result;
            var products = _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "Category,StockQuantities").Result;
            var categories = _unitOfWork.GetRepository<ProductCategory>().GetAll(false).Result;

            var productsVm = _mapper.Map<System.Collections.Generic.IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel>>(products).ToList();
            foreach (var p in productsVm)
            {
                var entity = products.FirstOrDefault(x => x.Id == p.Id);
                if (entity != null && entity.StockQuantities != null)
                {
                    p.StockByWarehouse = entity.StockQuantities
                        .GroupBy(sq => sq.WarehouseId)
                        .ToDictionary(g => g.Key, g => g.Sum(sq => sq.Quantity));
                        
                    p.AvailableQuantity = entity.StockQuantities.Sum(sq => sq.Quantity);
                }
            }

            return new StockAdjustmentViewModel
            {
                Warehouses = _mapper.Map<System.Collections.Generic.IEnumerable<WarehouseViewModel>>(warehouses).ToList(),
                Products = productsVm,
                Categories = _mapper.Map<System.Collections.Generic.IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductCategoryViewModel>>(categories).ToList(),
                AdjustmentItems = new System.Collections.Generic.List<StockAdjustmentItemViewModel>()
            };
        }

        public async Task CreateAsync(StockAdjustmentViewModel model)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(StockAdjustmentViewModel model)
        {
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            await Task.CompletedTask;
        }
    }
}
