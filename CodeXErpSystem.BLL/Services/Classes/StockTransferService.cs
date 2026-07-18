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
            var products = _unitOfWork.GetRepository<Product>().GetAll(false).Result;

            return new StockTransferViewModel
            {
                Warehouses = _mapper.Map<IEnumerable<WarehouseViewModel>>(warehouses).ToList(),
                Products = _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel>>(products).ToList(),
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
