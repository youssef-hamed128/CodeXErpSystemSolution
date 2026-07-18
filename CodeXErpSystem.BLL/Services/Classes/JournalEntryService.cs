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
    public class JournalEntryService : IJournalEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JournalEntryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JournalEntryViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<JournalEntry>().GetAll(false);
            return _mapper.Map<IEnumerable<JournalEntryViewModel>>(entities);
        }

        public async Task CreateAsync(JournalEntryViewModel model)
        {
            var entity = _mapper.Map<JournalEntry>(model);
            _unitOfWork.GetRepository<JournalEntry>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(JournalEntryViewModel model)
        {
            var entity = _mapper.Map<JournalEntry>(model);
            _unitOfWork.GetRepository<JournalEntry>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<JournalEntry>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
