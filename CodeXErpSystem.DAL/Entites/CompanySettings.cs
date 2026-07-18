using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class CompanySettings : BaseEntity
    {
        public string CompanyName { get; set; } = null!;
        public string? TaxNumber { get; set; }
        public string? CRNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string BaseCurrency { get; set; } = "EGP";
        public decimal? DefaultTaxRate { get; set; } = 0;
        public DateTime FinancialYearStart { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        public bool IsPortalTaxConnected { get; set; } = false;
        public string? PortalTaxAPIKey { get; set; }




    }
}
