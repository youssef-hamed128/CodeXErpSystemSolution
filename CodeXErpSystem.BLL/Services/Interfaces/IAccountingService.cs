using CodeXErpSystem.BLL.ViewModels.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IAccountingService
    {
        Task<JournalEntryViewModel> CreateJournalEntryAsync(JournalEntryCreateViewModel model, string UserId, CancellationToken ct = default);
    }
}
