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

            // ذمم مدينة = فواتير مبيعات غير مدفوعة
            ViewBag.TotalReceivable = salesInvoices
                .Where(i => i.Status != InvoiceStatus.Paid)
                .Sum(i => i.TotalAmount);

            // ذمم دائنة = فواتير مشتريات غير مدفوعة
            ViewBag.TotalPayable = purchaseInvoices
                .Where(i => i.Status != InvoiceStatus.Paid)
                .Sum(i => i.TotalAmount);

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
