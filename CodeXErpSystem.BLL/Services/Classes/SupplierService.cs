using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Suppliers;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            var existingName = await _unitOfWork.GetRepository<Supplier>().FindAsync(s => s.Name == model.Name);
            if (existingName.Any()) throw new System.InvalidOperationException("اسم المورد مكرر مسبقاً.");

            if (!string.IsNullOrEmpty(model.Phone))
            {
                var existingPhone = await _unitOfWork.GetRepository<Supplier>().FindAsync(s => s.Phone == model.Phone);
                if (existingPhone.Any()) throw new System.InvalidOperationException("رقم هاتف المورد مكرر مسبقاً.");
            }

            var entity = _mapper.Map<Supplier>(model);
            _unitOfWork.GetRepository<Supplier>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(SupplierViewModel model)
        {
            var existingName = await _unitOfWork.GetRepository<Supplier>().FindAsync(s => s.Name == model.Name && s.Id != model.Id);
            if (existingName.Any()) throw new System.InvalidOperationException("اسم المورد مكرر مع مورد آخر.");

            if (!string.IsNullOrEmpty(model.Phone))
            {
                var existingPhone = await _unitOfWork.GetRepository<Supplier>().FindAsync(s => s.Phone == model.Phone && s.Id != model.Id);
                if (existingPhone.Any()) throw new System.InvalidOperationException("رقم هاتف المورد مكرر مع مورد آخر.");
            }

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
