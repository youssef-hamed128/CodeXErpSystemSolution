using System;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class ZatcaTaxViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public decimal TotalSales { get; set; }
        public decimal OutputTax { get; set; }

        public decimal TotalPurchases { get; set; }
        public decimal InputTax { get; set; }

        public decimal NetTaxDue => OutputTax - InputTax;
    }
}
