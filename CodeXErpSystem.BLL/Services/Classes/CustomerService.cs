using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Customers;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CodeXErpSystem.BLL.ViewModels.Customers.CustomerViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<Customer>().GetAll(false);
            var entitiesList = entities.ToList();

            var cashCustomer = entitiesList.FirstOrDefault(c => c.Name == "نقدي");
            if (cashCustomer == null)
            {
                cashCustomer = new Customer
                {
                    Name = "نقدي",
                    Phone = "0000000000",
                    Address = "عميل نقدي افتراضي",
                    Balance = 0,
                    CreditLimit = 0,
                    IsDeleted = false
                };
                _unitOfWork.GetRepository<Customer>().Add(cashCustomer);
                await _unitOfWork.CompleteAsync();
                entitiesList.Add(cashCustomer);
            }

            var result = _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Customers.CustomerViewModel>>(entitiesList).ToList();
            for (int i = 0; i < entitiesList.Count; i++)
            {
                result[i].Balance = entitiesList[i].Balance ?? 0;
            }
            return result;
        }

        public async Task CreateAsync(CustomerViewModel model)
        {
            var existingName = await _unitOfWork.GetRepository<Customer>().FindAsync(c => c.Name == model.Name);
            if (existingName.Any()) throw new System.InvalidOperationException("اسم العميل مكرر مسبقاً.");

            if (!string.IsNullOrEmpty(model.Phone))
            {
                var existingPhone = await _unitOfWork.GetRepository<Customer>().FindAsync(c => c.Phone == model.Phone);
                if (existingPhone.Any()) throw new System.InvalidOperationException("رقم هاتف العميل مكرر مسبقاً.");
            }

            var entity = _mapper.Map<Customer>(model);
            _unitOfWork.GetRepository<Customer>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(CustomerViewModel model)
        {
            var existingTarget = await _unitOfWork.GetRepository<Customer>().GetById(model.Id);
            if (existingTarget != null && existingTarget.Name == "نقدي" && model.Name != "نقدي")
            {
                throw new System.InvalidOperationException("لا يمكن تغيير اسم العميل الافتراضي (نقدي).");
            }

            var existingName = await _unitOfWork.GetRepository<Customer>().FindAsync(c => c.Name == model.Name && c.Id != model.Id);
            if (existingName.Any()) throw new System.InvalidOperationException("اسم العميل مكرر مع عميل آخر.");

            if (!string.IsNullOrEmpty(model.Phone))
            {
                var existingPhone = await _unitOfWork.GetRepository<Customer>().FindAsync(c => c.Phone == model.Phone && c.Id != model.Id);
                if (existingPhone.Any()) throw new System.InvalidOperationException("رقم هاتف العميل مكرر مع عميل آخر.");
            }

            var entity = _mapper.Map<Customer>(model);
            _unitOfWork.GetRepository<Customer>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var target = await _unitOfWork.GetRepository<Customer>().GetById(id);
            if (target != null && target.Name == "نقدي")
            {
                throw new System.InvalidOperationException("لا يمكن حذف العميل الافتراضي (نقدي).");
            }

            _unitOfWork.GetRepository<Customer>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
