using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class Payment : BaseEntity
    {
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Reference { get; set; } 
        public Invoice? Invoice { get; set; } = null!;
        [ForeignKey("InvoiceId")]
        public int InvoiceId { get; set; }
        public Customer? Customer { get; set; } = null!;
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public Supplier? Supplier { get; set; } = null!;
        [ForeignKey("SupplierId")]
        public int SupplierId { get; set; }



    }
}
