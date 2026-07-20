using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Invoice
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public InvoiceType Type { get; set; }
        public string TypeDisplay => Type switch
        {
            InvoiceType.Sales => "فاتورة مبيعات",
            InvoiceType.Purchase => "فاتورة مشتريات",
            InvoiceType.SalesReturn => "مرتجع مبيعات",
            InvoiceType.PurchaseReturn => "مرتجع مشتريات",
            _ => Type.ToString()
        };
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal SubTotal { get; set; }

        // الخصومات مضافة هنا!
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string StatusDisplay => Status switch
        {
            InvoiceStatus.Unpaid => "غير مدفوعة",
            InvoiceStatus.Partial => "مدفوعة جزئياً",
            InvoiceStatus.Paid => "مدفوعة بالكامل",
            _ => Status.ToString()
        };
        public string? Notes { get; set; }
        public string? AttachmentUrl { get; set; }
        public List<InvoiceItemViewModel> Items { get; set; } = new();
    }
}
