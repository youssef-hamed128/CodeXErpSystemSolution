using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels.Accounting;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<Account>().GetAll(false);
            return _mapper.Map<IEnumerable<AccountViewModel>>(entities);
        }

        public async Task CreateAsync(AccountViewModel model)
        {
            var entity = _mapper.Map<Account>(model);
            _unitOfWork.GetRepository<Account>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(AccountViewModel model)
        {
            var entity = _mapper.Map<Account>(model);
            _unitOfWork.GetRepository<Account>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Account>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
