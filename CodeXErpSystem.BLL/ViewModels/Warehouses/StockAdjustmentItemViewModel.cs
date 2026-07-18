using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class StockAdjustmentItemViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal Difference { get; set; }
    }
}
