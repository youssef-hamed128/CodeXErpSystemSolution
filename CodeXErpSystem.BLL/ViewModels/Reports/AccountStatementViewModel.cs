using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class AccountStatementViewModel
    {
        public int PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<StatementItemViewModel> Items { get; set; } = new();
    }
}
