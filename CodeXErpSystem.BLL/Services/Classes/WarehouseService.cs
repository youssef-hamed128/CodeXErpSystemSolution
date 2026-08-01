using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Warehouses;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<WarehouseViewModel> GetAllWarehouses()
        {
            var entities = _unitOfWork.GetRepository<Warehouse>().FindAsync(includeProperties: "StockQuantities,StockQuantities.Product").Result;
            var viewModels = _mapper.Map<IEnumerable<WarehouseViewModel>>(entities).ToList();

            foreach (var vm in viewModels)
            {
                var entity = entities.FirstOrDefault(e => e.Id == vm.Id);
                if (entity != null && entity.StockQuantities != null)
                {
                    vm.TotalQuantity = entity.StockQuantities.Sum(sq => sq.Quantity);
                    vm.TotalInventoryValue = entity.StockQuantities.Sum(sq => sq.Quantity * (sq.Product?.PurchasePrice ?? 0));
                }
            }

            return viewModels;
        }

        public async Task CreateAsync(WarehouseViewModel model)
        {
            var entity = _mapper.Map<Warehouse>(model);
            _unitOfWork.GetRepository<Warehouse>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(WarehouseViewModel model)
        {
            var entity = _mapper.Map<Warehouse>(model);
            _unitOfWork.GetRepository<Warehouse>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Warehouse>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
