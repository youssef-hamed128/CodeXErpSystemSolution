using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class DebtsReportViewModel
    {
        public List<CustomerDebtItem> Customers { get; set; } = new();
        public List<SupplierDebtItem> Suppliers { get; set; } = new();

        public decimal TotalCustomerDebts { get; set; }
        public decimal TotalSupplierPayables { get; set; }
        public decimal NetBalance => TotalCustomerDebts - TotalSupplierPayables;
    }

    public class CustomerDebtItem
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public decimal TotalInvoices { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class SupplierDebtItem
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public decimal TotalInvoices { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
