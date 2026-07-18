using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Warehouses;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IStockTransferService
    {
        StockTransferViewModel GetStockTransferInitialData();
        Task CreateAsync(StockTransferViewModel model);
        Task UpdateAsync(StockTransferViewModel model);
        Task DeleteAsync(int id);
    }
}
