using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Expenses
{
    public class ExpenseCreateViewModel
    {
        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن تكون القيمة أكبر من صفر")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "التصنيف مطلوب")]
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public string PaymentMethod { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
    }
}

