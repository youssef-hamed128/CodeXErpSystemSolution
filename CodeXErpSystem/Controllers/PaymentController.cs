using Microsoft.AspNetCore.Authorization;
using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, محاسب")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly CodeXErpSystem.BLL.Services.Interfaces.ICustomerService _customerService;
        private readonly CodeXErpSystem.BLL.Services.Interfaces.ISupplierService _supplierService;
        private readonly CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork _unitOfWork;

        public PaymentController(
            IPaymentService paymentService, 
            CodeXErpSystem.BLL.Services.Interfaces.ICustomerService customerService,
            CodeXErpSystem.BLL.Services.Interfaces.ISupplierService supplierService,
            CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _customerService = customerService;
            _supplierService = supplierService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _paymentService.GetAllAsync();
            ViewBag.Customers = await _customerService.GetAllAsync();
            ViewBag.Suppliers = await _supplierService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeXErpSystem.BLL.ViewModels.Payments.PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _paymentService.CreateAsync(model);
                return Json(new { success = true, message = "تم إضافة السند بنجاح" });
            }
            var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return Json(new { success = false, message = "خطأ في البيانات: " + errors });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CodeXErpSystem.BLL.ViewModels.Payments.PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _paymentService.UpdateAsync(model);
                return Json(new { success = true, message = "تم تعديل السند بنجاح" });
            }
            var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return Json(new { success = false, message = "خطأ في البيانات: " + errors });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _paymentService.DeleteAsync(id);
            return Json(new { success = true, message = "Payment deleted successfully" });
        }

        public async Task<IActionResult> Print(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound("السند غير موجود");
            return View(payment);
        }

        [HttpGet]
        public async Task<IActionResult> GetPartyBalanceInfo(int id, string type)
        {
            if (type == "Customer")
            {
                var customerList = await _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.Customer>().FindAsync(c => c.Id == id, includeProperties: "Invoices");
                var c = customerList.FirstOrDefault();
                if (c == null) return NotFound();

                var salesInvoices = c.Invoices.Where(i => i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Sales).ToList();
                decimal requiredAmount = salesInvoices.Where(i => i.Status != CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Paid).Sum(i => i.TotalAmount - i.PaidAmount);
                decimal currentBalance = c.Balance ?? 0;

                return Json(new
                {
                    success = true,
                    balance = currentBalance,
                    requiredAmount = requiredAmount
                });
            }
            else
            {
                var supplierList = await _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.Supplier>().FindAsync(s => s.Id == id, includeProperties: "Invoices");
                var s = supplierList.FirstOrDefault();
                if (s == null) return NotFound();

                var purchaseInvoices = s.Invoices.Where(i => i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Purchase).ToList();
                decimal requiredAmount = purchaseInvoices.Where(i => i.Status != CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Paid).Sum(i => i.TotalAmount - i.PaidAmount);
                decimal currentBalance = s.Balance ?? 0;

                return Json(new
                {
                    success = true,
                    balance = currentBalance,
                    requiredAmount = requiredAmount
                });
            }
        }
    }
}

