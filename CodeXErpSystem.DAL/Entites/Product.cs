using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class Product : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProductCategory Category { get; set; } = null!;
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? TaxRate { get; set; }
        public bool HasStockTracking { get; set; } = false;
        public string? UnitOfMeasure { get; set; }
        public ICollection<StockQuantity> StockQuantities { get; set; } = new List<StockQuantity>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    }
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
