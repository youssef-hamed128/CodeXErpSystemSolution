using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class CustomerStatementViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public decimal CustomerCurrentBalance { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal TotalInvoicesAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRemainingAmount { get; set; }

        public List<StatementInvoiceItem> Invoices { get; set; } = new();
        public List<StatementPaymentItem> Receipts { get; set; } = new();
    }

    public class StatementInvoiceItem
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class StatementPaymentItem
    {
        public int PaymentId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethodName { get; set; } = string.Empty;
        public string? LinkedInvoiceNumber { get; set; }
        public string? Reference { get; set; }
    }
}
