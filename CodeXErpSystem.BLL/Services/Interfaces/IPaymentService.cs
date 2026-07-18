using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentViewModel>> GetAllAsync();
        Task CreateAsync(PaymentViewModel model);
        Task UpdateAsync(PaymentViewModel model);
        Task DeleteAsync(int id);
    }
}
