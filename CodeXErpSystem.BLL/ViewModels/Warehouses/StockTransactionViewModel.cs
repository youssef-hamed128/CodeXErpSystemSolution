using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class StockTransactionViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? SourceWarehouseName { get; set; }
        public string? DestWarehouseName { get; set; }
        public decimal Quantity { get; set; }
        public string? ReferenceId { get; set; }
        public string? Notes { get; set; }
    }
}
