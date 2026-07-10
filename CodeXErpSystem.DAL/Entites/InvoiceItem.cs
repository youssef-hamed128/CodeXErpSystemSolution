using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class InvoiceItem : BaseEntity
    {
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public Product Product { get; set; } = null!;
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        [ForeignKey("InvoiceId")]
        public int InvoiceId { get; set; }

    }
}
