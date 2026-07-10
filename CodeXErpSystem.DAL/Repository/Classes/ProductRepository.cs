using CodeXErpSystem.DAL.Contexts;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Classes
{
    public class ProductRepository : GenaricRepository<Product>, IProductRepository
    {
        private readonly CodeXDbContext dbContext;

        public ProductRepository(CodeXDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<Product>> GetLowStockProductAsync(CancellationToken ct = default)
        {
            return await dbContext.Products.Where(p=>p.HasStockTracking && p.StockQuantities.Sum(sq => sq.Quantity) <= 1) .ToListAsync(ct);

        }

        public async Task<decimal> GetProductStockInWarehouseAsync(int productId, int warehouseId , CancellationToken ct = default)
        {
            var Stock = await dbContext.StockQuantities.FirstOrDefaultAsync(sq => sq.ProductId == productId && sq.WarehouseId == warehouseId, ct);
            return Stock?.Quantity ?? 0;
        }
    }
}
