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
            return _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Customers.CustomerViewModel>>(entities);
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
            _unitOfWork.GetRepository<Customer>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
