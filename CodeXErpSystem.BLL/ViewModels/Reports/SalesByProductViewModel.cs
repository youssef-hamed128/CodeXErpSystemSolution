using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class SalesByProductViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProductSalesItem> Products { get; set; } = new List<ProductSalesItem>();
    }

    public class ProductSalesItem
    {
        public string Code { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
