using Microsoft.AspNetCore.Mvc;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Warehouses;

namespace CodeXErpSystem.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public IActionResult Index()
        {
            var warehouses = _warehouseService.GetAllWarehouses();
            return View(warehouses);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create(WarehouseViewModel model)
        {
            try
            {
                await _warehouseService.CreateAsync(model);
                return Json(new { success = true, message = "Warehouse created successfully." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WarehouseViewModel model)
        {
            try
            {
                await _warehouseService.UpdateAsync(model);
                return Json(new { success = true, message = "Warehouse updated successfully." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public async    Task<IActionResult> Delete(int id)
        {
            await _warehouseService.DeleteAsync(id);
            return Json(new { success = true, message = "Warehouse deleted successfully." });
        }
    }
}
