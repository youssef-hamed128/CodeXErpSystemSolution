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

        public PaymentController(
            IPaymentService paymentService, 
            CodeXErpSystem.BLL.Services.Interfaces.ICustomerService customerService,
            CodeXErpSystem.BLL.Services.Interfaces.ISupplierService supplierService)
        {
            _paymentService = paymentService;
            _customerService = customerService;
            _supplierService = supplierService;
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
    }
}

