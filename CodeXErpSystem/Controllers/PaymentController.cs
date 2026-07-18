using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _paymentService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeXErpSystem.BLL.ViewModels.Payments.PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _paymentService.CreateAsync(model);
                return Json(new { success = true, message = "Payment created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CodeXErpSystem.BLL.ViewModels.Payments.PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _paymentService.UpdateAsync(model);
                return Json(new { success = true, message = "Payment updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _paymentService.DeleteAsync(id);
            return Json(new { success = true, message = "Payment deleted successfully" });
        }
    }
}
