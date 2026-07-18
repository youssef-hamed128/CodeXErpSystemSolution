using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Payments
{
    public class PaymentCreateViewModel
    {
        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن تكون القيمة أكبر من صفر")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public PaymentMethod PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public int? InvoiceId { get; set; }
    }
}

