using AutoMapper;
using CodeXErpSystem.BLL.ViewModels.Invoice;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Entites.Enums;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            // ===== جلب كل الفواتير مرة واحدة =====
            var allInvoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                includeProperties: "Customer,Supplier");

            var salesInvoices    = allInvoices.Where(i => i.Type == InvoiceType.Sales).ToList();
            var purchaseInvoices = allInvoices.Where(i => i.Type == InvoiceType.Purchase).ToList();

            // ===== KPIs =====
            ViewBag.TotalSales     = salesInvoices.Sum(i => i.TotalAmount);
            ViewBag.TotalPurchases = purchaseInvoices.Sum(i => i.TotalAmount);
            ViewBag.SalesCount     = salesInvoices.Count;
            ViewBag.PurchaseCount  = purchaseInvoices.Count;

            // ذمم مدينة = المتبقي من إجمالي فواتير البيع
            ViewBag.TotalReceivable = salesInvoices
                .Sum(i => Math.Max(0, i.TotalAmount - i.PaidAmount));

            // ذمم دائنة = المتبقي من إجمالي فواتير المشتريات
            ViewBag.TotalPayable = purchaseInvoices
                .Sum(i => Math.Max(0, i.TotalAmount - i.PaidAmount));

            // حساب بيانات الرسم البياني (آخر 6 أشهر)
            string[] arabicMonths = { "", "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Today.AddMonths(-5 + i))
                .ToList();

            var labels = last6Months.Select(d => arabicMonths[d.Month]).ToList();
            var salesData = last6Months.Select(d => salesInvoices.Where(i => i.Date.Year == d.Year && i.Date.Month == d.Month).Sum(i => i.TotalAmount)).ToList();
            var purchaseData = last6Months.Select(d => purchaseInvoices.Where(i => i.Date.Year == d.Year && i.Date.Month == d.Month).Sum(i => i.TotalAmount)).ToList();

            ViewBag.ChartLabels = System.Text.Json.JsonSerializer.Serialize(labels);
            ViewBag.ChartSalesData = System.Text.Json.JsonSerializer.Serialize(salesData);
            ViewBag.ChartPurchasesData = System.Text.Json.JsonSerializer.Serialize(purchaseData);

            // أحدث 8 عمليات
            var recent = allInvoices
                .OrderByDescending(i => i.Id)
                .Take(8)
                .ToList();

            ViewBag.RecentInvoices = _mapper.Map<IEnumerable<InvoiceViewModel>>(recent);

            return View();
        }
    }
}
