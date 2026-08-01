using System.Collections.Generic;
using System.Linq;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class InventoryReportViewModel
    {
        public decimal TotalInventoryValue { get; set; }
        public decimal TotalSoldQuantity => Items.Sum(i => i.SoldQuantity);
        public decimal TotalCurrentQuantity => Items.Sum(i => i.Quantity);
        public List<InventoryItemReport> Items { get; set; } = new List<InventoryItemReport>();
    }

    public class InventoryItemReport
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = "المخزن الرئيسي";
        public decimal SoldQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue => Quantity * UnitCost;
    }
}
