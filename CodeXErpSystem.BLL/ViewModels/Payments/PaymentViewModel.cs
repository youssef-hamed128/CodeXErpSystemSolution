using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Payments
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }


    }
}
