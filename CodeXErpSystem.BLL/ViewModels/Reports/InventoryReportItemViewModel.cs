using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class InventoryReportItemViewModel
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string? UnitOfMeasure { get; set; }
        public bool IsLowStock { get; set; }
    }
}
