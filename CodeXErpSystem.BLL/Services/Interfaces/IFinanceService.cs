using CodeXErpSystem.BLL.ViewModels.Expenses;
using CodeXErpSystem.BLL.ViewModels.Payments;
using CodeXErpSystem.DAL.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IFinanceService
    {
        Task<PaymentViewModel> CreatPaymentViewModel(PaymentCreateViewModel model, string UserId, CancellationToken ct = default);
        Task<ExpenseViewModel> CreatExpenseAsync(ExpenseCreateViewModel model, string UserId, CancellationToken ct = default);
        Task<String> GenerateReceiptNumberAsync();
    }
}
