using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class SalesByCustomerViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CustomerSalesItem> Customers { get; set; } = new List<CustomerSalesItem>();
    }

    public class CustomerSalesItem
    {
        public string CustomerName { get; set; } = string.Empty;
        public int InvoicesCount { get; set; }
        public decimal TotalSales { get; set; }
    }
}
