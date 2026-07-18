using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Suppliers;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CodeXErpSystem.BLL.ViewModels.Suppliers.SupplierViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<Supplier>().GetAll(false);
            return _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Suppliers.SupplierViewModel>>(entities);
        }

        public async Task CreateAsync(SupplierViewModel model)
        {
            var entity = _mapper.Map<Supplier>(model);
            _unitOfWork.GetRepository<Supplier>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(SupplierViewModel model)
        {
            var entity = _mapper.Map<Supplier>(model);
            _unitOfWork.GetRepository<Supplier>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Supplier>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
