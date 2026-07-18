using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Warehouses;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IWarehouseService
    {
        IEnumerable<WarehouseViewModel> GetAllWarehouses();
        Task CreateAsync(WarehouseViewModel model);
        Task UpdateAsync(WarehouseViewModel model);
        Task DeleteAsync(int id);
    }
}
