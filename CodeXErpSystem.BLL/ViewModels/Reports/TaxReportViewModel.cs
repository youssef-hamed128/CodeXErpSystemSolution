using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class TaxReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSalesRevenue { get; set; }
        public decimal TotalSalesTaxCollected { get; set; }
        public decimal TotalPurchasesCost { get; set; }
        public decimal TotalPurchaseTaxPaid { get; set; }
        public decimal NetTaxDue => TotalSalesTaxCollected - TotalPurchaseTaxPaid;


    }
}
