using CodeXErpSystem.BLL.ViewModels.Invoice;
using CodeXErpSystem.BLL.ViewModels.Payments;
using CodeXErpSystem.BLL.ViewModels.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Reports
{
    public class DashboardViewModel
    {
        public decimal TotalSalesThisMonth { get; set; }
        public decimal TotalPurchasesThisMonth { get; set; }
        public decimal TotalExpensesThisMonth { get; set; }
        public int LowStockProductsCount { get; set; }
        public decimal TotalReceivables { get; set; } // إجمالي مديونيات العملاء
        public decimal TotalPayables { get; set; }    // إجمالي مديونيات الموردين
        public List<InvoiceViewModel> RecentInvoices { get; set; } = new();
        public List<PaymentViewModel> RecentPayments { get; set; } = new();
        public List<WarehouseViewModel> LowStockItems { get; set; } = new();
        // بيانات الرسم البياني (Chart.js)
        public List<decimal> MonthlySalesValues { get; set; } = new();
        public List<decimal> MonthlyPurchasesValues { get; set; } = new();
        public List<string> MonthlyLabels { get; set; } = new();
    }
}
