using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Accounting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountViewModel>> GetAllAsync();
        Task CreateAsync(AccountViewModel model);
        Task UpdateAsync(AccountViewModel model);
        Task DeleteAsync(int id);
    }
}
