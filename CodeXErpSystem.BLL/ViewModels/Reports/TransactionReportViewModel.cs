using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class TransactionReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? PartyId { get; set; }
        public int? ProductId { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TransactionReportItemViewModel> Items { get; set; } = new();
    }
}
