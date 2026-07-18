using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels.Payments;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<Payment>().GetAll(false);
            return _mapper.Map<IEnumerable<PaymentViewModel>>(entities);
        }

        public async Task CreateAsync(PaymentViewModel model)
        {
            var entity = _mapper.Map<Payment>(model);
            _unitOfWork.GetRepository<Payment>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(PaymentViewModel model)
        {
            var entity = _mapper.Map<Payment>(model);
            _unitOfWork.GetRepository<Payment>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Payment>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
