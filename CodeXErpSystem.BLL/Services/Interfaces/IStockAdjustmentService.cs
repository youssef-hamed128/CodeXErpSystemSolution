using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Warehouses;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IStockAdjustmentService
    {
        StockAdjustmentViewModel GetStockAdjustmentInitialData();
        Task CreateAsync(StockAdjustmentViewModel model);
        Task UpdateAsync(StockAdjustmentViewModel model);
        Task DeleteAsync(int id);
    }
}
