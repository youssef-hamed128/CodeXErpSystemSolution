using CodeXErpSystem.BLL.ViewModels.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceViewModel> CreateInvoiceAsync(InvoiceCreateViewModel model,string userId, CancellationToken ct = default);
        Task<string> GenerateInvoiceNumberAsync(CodeXErpSystem.DAL.Entites.Enums.InvoiceType type);
        Task<InvoiceViewModel?> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken ct = default);
        Task<bool> UpdateInvoiceStatusAsync(int invoiceId, CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus newStatus, CancellationToken ct = default);

    }
}
