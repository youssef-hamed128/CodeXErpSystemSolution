using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class ProfitAndLossViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSalesRevenue { get; set; }
        public decimal CostOfGoodsSold { get; set; }
        public decimal GrossProfit => TotalSalesRevenue - CostOfGoodsSold;
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit => GrossProfit - TotalExpenses;
    }
}
