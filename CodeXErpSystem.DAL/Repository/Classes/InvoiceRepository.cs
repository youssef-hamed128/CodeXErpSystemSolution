using CodeXErpSystem.DAL.Contexts;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Entites.Enums;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Classes
{
    public class InvoiceRepository : GenaricRepository<Invoice>, IInvoiceRepository
    {
        private readonly CodeXDbContext dbContext;

        public InvoiceRepository(CodeXDbContext dbContext) : base (dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> GenerateInvoiceNumberAsync(InvoiceType type, CancellationToken ct = default)
        {
            var year = DateTime.UtcNow.Year;
            var prefix = type == InvoiceType.Sales ? "INV" : "PUR";
            // الحصول على آخر رقم فاتورة تم إنشاؤه لهذا النوع في هذا العام
            var lastInvoice = await dbContext.Invoices
                .Where(i => i.Type == type && i.Date.Year == year)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync(ct);
            int nextSequence = 1;
            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                {
                    nextSequence = lastSeq + 1;
                }
            }
            return $"{prefix}-{year}-{nextSequence.ToString("D4")}"; // ينتج: INV-2026-0005
        }
  
        

        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int invoiceId, CancellationToken ct = default)
        {
            return await dbContext.Invoices.Include(i => i.Items).ThenInclude(item => item.Product).Include(i => i.Customer).Include(i => i.Supplier)
                                          .FirstOrDefaultAsync(i => i.Id == invoiceId, ct);
        }
    }
}
