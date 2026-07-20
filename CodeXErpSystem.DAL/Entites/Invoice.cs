using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class Invoice : BaseEntity
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public InvoiceType Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? Note { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal DiscountPercentage { get; set; } = 0;
        public Customer? Customer { get; set; }
        [ForeignKey("CustomerId")]
        public int? CustomerId { get; set; }
        public Supplier? Supplier { get; set; }
        [ForeignKey("SupplierId")]
        public int? SupplierId { get; set; }
        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
