using Microsoft.AspNetCore.Authorization;
using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, مشتريات ومخازن")]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index() { var model = await _supplierService.GetAllAsync(); ViewBag.TotalSuppliers = model.Count(); ViewBag.ActiveSuppliers = model.Count(); ViewBag.TotalBalance = model.Sum(s => s.Balance); return View(model); }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeXErpSystem.BLL.ViewModels.Suppliers.SupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierService.CreateAsync(model);
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
        public async Task<IActionResult> Edit([FromForm] CodeXErpSystem.BLL.ViewModels.Suppliers.SupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierService.UpdateAsync(model);
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
            await _supplierService.DeleteAsync(id);
            return Json(new { success = true, message = "Supplier deleted successfully" });
        }
    }
}


