using CodeXErpSystem.BLL.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IReportService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(CancellationToken ct = default);


    }
}
