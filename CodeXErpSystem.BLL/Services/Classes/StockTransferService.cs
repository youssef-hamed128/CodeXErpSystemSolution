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
            // Dummy implementation to satisfy interface
            await Task.CompletedTask;
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
