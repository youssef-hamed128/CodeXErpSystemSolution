using CodeXErpSystem.BLL.ViewModels.Customers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllAsync();
        Task CreateAsync(CustomerViewModel model);
        Task UpdateAsync(CustomerViewModel model);
        Task DeleteAsync(int id);
    }
}
