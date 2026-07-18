using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Accounting
{
    public class JournalEntryLineViewModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
    }
}
