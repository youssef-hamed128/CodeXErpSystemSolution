using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace CodeXErpSystem.Controllers
{
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
                await _supplierService.CreateAsync(model);
                return Json(new { success = true, message = "Supplier created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CodeXErpSystem.BLL.ViewModels.Suppliers.SupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.UpdateAsync(model);
                return Json(new { success = true, message = "Supplier updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _supplierService.DeleteAsync(id);
            return Json(new { success = true, message = "Supplier deleted successfully" });
        }
    }
}

