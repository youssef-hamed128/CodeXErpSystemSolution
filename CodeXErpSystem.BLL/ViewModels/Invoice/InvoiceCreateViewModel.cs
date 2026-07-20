using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Invoice
{
    public class InvoiceCreateViewModel
    {
        public string? InvoiceNumber { get; set; }
        public string? ReferenceNumber { get; set; }

        [Required(ErrorMessage = "نوع الفاتورة مطلوب")]
        public InvoiceType Type { get; set; }
        [Required(ErrorMessage = "تاريخ الفاتورة مطلوب")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "تاريخ الاستحقاق مطلوب")]
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        [Required(ErrorMessage = "المخزن مطلوب")]
        public int WarehouseId { get; set; }
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercentage { get; set; } = 15;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? Notes { get; set; }
        public string? AttachmentUrl { get; set; }
        public List<InvoiceItemCreateViewModel> Items { get; set; } = new();
    }
}
