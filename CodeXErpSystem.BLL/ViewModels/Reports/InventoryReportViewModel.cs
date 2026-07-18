using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class InventoryReportViewModel
    {
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public List<InventoryReportItemViewModel> Items { get; set; } = new();
    }
}
