using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class InventoryReportViewModel
    {
        public decimal TotalInventoryValue { get; set; }
        public List<InventoryItemReport> Items { get; set; } = new List<InventoryItemReport>();
    }

    public class InventoryItemReport
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue => Quantity * UnitCost;
    }
}
