using CodeXErpSystem.BLL.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Invoice
{
    public class InvoiceItemCreateViewModel
    {
        [Required(ErrorMessage = "الصنف مطلوب")]
        public int ProductId { get; set; }
        [Range(0.0001, double.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون موجباً")]
        public decimal UnitPrice { get; set; }
    }
    // ============================================
    // نموذج مخصص لطباعة وعرض الفاتورة بالكامل (ببيانات الشركة)
    // ============================================
    public class InvoicePrintViewModel
    {
        public InvoiceViewModel Invoice { get; set; } = new();
        public CompanySettingsViewModel CompanyInfo { get; set; } = new();
        public string? PortalQR { get; set; }
    }
}

