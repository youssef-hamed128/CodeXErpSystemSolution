using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Accounting
{
    public class JournalEntryViewModel
    {
        public int Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public JournalEntryStatus Status { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public List<JournalEntryLineViewModel> Lines { get; set; } = new();
    }
}
