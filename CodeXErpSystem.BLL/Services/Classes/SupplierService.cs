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
            var cashSupplier = (await _unitOfWork.GetRepository<Supplier>().FindAsync(s => s.Name == "مورد نقدي" && !s.IsDeleted)).FirstOrDefault();
            if (cashSupplier == null)
            {
                cashSupplier = new Supplier
                {
                    Name = "مورد نقدي",
                    Phone = "-",
                    Email = "cash@supplier.com",
                    Address = "مورد نقدي",
                    Balance = 0,
                    CreatedBy = "System"
                };
                _unitOfWork.GetRepository<Supplier>().Add(cashSupplier);
                await _unitOfWork.CompleteAsync();
            }

            var entities = await _unitOfWork.GetRepository<Supplier>().GetAll(false);
            var result = _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Suppliers.SupplierViewModel>>(entities).ToList();
            var entitiesList = entities.ToList();
            for (int i = 0; i < entitiesList.Count; i++)
            {
                result[i].Balance = entitiesList[i].Balance ?? 0;
            }
            return result;
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
            var currentSupplier = await _unitOfWork.GetRepository<Supplier>().GetById(model.Id);
            if (currentSupplier != null && currentSupplier.Name == "مورد نقدي" && model.Name != "مورد نقدي")
            {
                throw new System.InvalidOperationException("لا يمكن تغيير اسم المورد النقدي الثابت للنظام.");
            }

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
            var supplier = await _unitOfWork.GetRepository<Supplier>().GetById(id);
            if (supplier != null && supplier.Name == "مورد نقدي")
            {
                throw new System.InvalidOperationException("لا يمكن حذف المورد النقدي الثابت للنظام.");
            }
            _unitOfWork.GetRepository<Supplier>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
