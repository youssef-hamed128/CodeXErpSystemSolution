using Microsoft.AspNetCore.Authorization;
using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, مبيعات")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index() { var model = await _customerService.GetAllAsync(); ViewBag.TotalCustomers = model.Count(); ViewBag.ActiveCustomers = model.Count(); ViewBag.TotalBalance = model.Sum(c => c.Balance); return View(model); }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeXErpSystem.BLL.ViewModels.Customers.CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _customerService.CreateAsync(model);
                    return Json(new { success = true, message = "تمت الإضافة بنجاح" });
                }
                catch (System.InvalidOperationException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "بيانات غير صالحة" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CodeXErpSystem.BLL.ViewModels.Customers.CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _customerService.UpdateAsync(model);
                    return Json(new { success = true, message = "تم التعديل بنجاح" });
                }
                catch (System.InvalidOperationException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "بيانات غير صالحة" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);
            return Json(new { success = true, message = "Customer deleted successfully" });
        }
    }
}


