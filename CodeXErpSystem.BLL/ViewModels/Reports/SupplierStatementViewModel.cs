using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class SupplierStatementViewModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public decimal SupplierCurrentBalance { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal TotalInvoicesAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRemainingAmount { get; set; }

        public List<StatementInvoiceItem> Invoices { get; set; } = new();
        public List<StatementPaymentItem> Payments { get; set; } = new();
    }
}
