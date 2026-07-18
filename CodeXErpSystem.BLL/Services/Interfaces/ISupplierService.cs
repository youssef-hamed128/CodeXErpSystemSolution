using CodeXErpSystem.BLL.ViewModels.Suppliers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierViewModel>> GetAllAsync();
        Task CreateAsync(SupplierViewModel model);
        Task UpdateAsync(SupplierViewModel model);
        Task DeleteAsync(int id);
    }
}
