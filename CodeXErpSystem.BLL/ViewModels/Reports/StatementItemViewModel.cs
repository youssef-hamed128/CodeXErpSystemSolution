using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class StatementItemViewModel
    {
        public DateTime Date { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
