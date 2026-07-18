using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Accounting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IJournalEntryService
    {
        Task<IEnumerable<JournalEntryViewModel>> GetAllAsync();
        Task CreateAsync(JournalEntryViewModel model);
        Task UpdateAsync(JournalEntryViewModel model);
        Task DeleteAsync(int id);
    }
}
