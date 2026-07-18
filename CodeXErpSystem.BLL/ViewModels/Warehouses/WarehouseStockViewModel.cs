using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class WarehouseStockViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal MinStockLevel { get; set; }
        public bool IsLowStock => Quantity <= MinStockLevel;
        public string? UnitOfMeasure { get; set; }
    }
}
