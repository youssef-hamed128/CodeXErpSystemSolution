using CodeXErpSystem.BLL.ViewModels.Accounting;
using System;
using System.Collections.Generic;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class IncomeStatementViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public decimal TotalRevenues { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome => TotalRevenues - TotalExpenses;

        public List<AccountViewModel> RevenueAccounts { get; set; } = new List<AccountViewModel>();
        public List<AccountViewModel> ExpenseAccounts { get; set; } = new List<AccountViewModel>();
    }
}
