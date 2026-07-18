using System.Collections.Generic;
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
            var entities = _unitOfWork.GetRepository<Warehouse>().GetAll(false).Result;
            return _mapper.Map<IEnumerable<WarehouseViewModel>>(entities);
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
