using CodeXErpSystem.BLL.ViewModels.Products;
using CodeXErpSystem.BLL.ViewModels.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<bool> TransferStockAsync(StockTransferViewModel model, string UserId, CancellationToken ct = default);
        Task<IEnumerable<ProductViewModel>> GetLowStockProductsAsync(CancellationToken ct = default);
    }
}
