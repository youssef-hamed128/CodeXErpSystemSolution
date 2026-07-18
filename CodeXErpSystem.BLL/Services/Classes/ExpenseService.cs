using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels.Expenses;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExpenseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExpenseViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<Expense>().GetAll(false);
            return _mapper.Map<IEnumerable<ExpenseViewModel>>(entities);
        }

        public async Task CreateAsync(ExpenseViewModel model)
        {
            var entity = _mapper.Map<Expense>(model);
            _unitOfWork.GetRepository<Expense>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(ExpenseViewModel model)
        {
            var entity = _mapper.Map<Expense>(model);
            _unitOfWork.GetRepository<Expense>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Expense>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
