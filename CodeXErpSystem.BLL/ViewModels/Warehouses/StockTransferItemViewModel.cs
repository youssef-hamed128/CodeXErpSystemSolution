using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class StockTransferItemViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal TransferredQuantity { get; set; }
    }
}
