using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Inetrfaces
{
    public interface IInvoiceRepository : IGenaricRepository<Invoice>
    {
        Task<Invoice?> GetInvoiceWithDetailsAsync(int invoiceId, CancellationToken ct = default);
        Task<string> GenerateInvoiceNumberAsync(InvoiceType type, CancellationToken ct = default);

    }
}
