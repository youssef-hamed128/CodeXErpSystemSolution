using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class TrialBalanceViewModel
    {
        public DateTime AsOfDate { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public bool IsBalanced => TotalDebit == TotalCredit;

        public List<TrialBalanceItem> Accounts { get; set; } = new List<TrialBalanceItem>();
    }

    public class TrialBalanceItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
