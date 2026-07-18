using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Expenses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseViewModel>> GetAllAsync();
        Task CreateAsync(ExpenseViewModel model);
        Task UpdateAsync(ExpenseViewModel model);
        Task DeleteAsync(int id);
    }
}
