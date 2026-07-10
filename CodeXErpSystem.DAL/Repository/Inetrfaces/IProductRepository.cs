using CodeXErpSystem.DAL.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Inetrfaces
{
    public interface IProductRepository : IGenaricRepository<Product>
    {
        Task<decimal> GetProductStockInWarehouseAsync(int productId, int warehouseId , CancellationToken ct = default);
        Task<IEnumerable<Product>> GetLowStockProductAsync(CancellationToken ct = default);


    }
}
