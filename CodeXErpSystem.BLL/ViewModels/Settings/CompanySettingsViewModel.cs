using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Settings
{
    public class CompanySettingsViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم الشركة مطلوب")]
        public string CompanyName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? CommercialRegistration { get; set; }
        public string? Phone { get; set; }
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string? Email { get; set; }
        public string? NationalAddress { get; set; }
        public string? LogoPath { get; set; }
        public string BaseCurrency { get; set; } = "SAR";
        public decimal DefaultTaxRate { get; set; } = 15;
        public DateTime FinancialYearStart { get; set; } = DateTime.UtcNow;
        public bool IsZatcaConnected { get; set; }
        public string? ZatcaApiKey { get; set; }
    }
}
