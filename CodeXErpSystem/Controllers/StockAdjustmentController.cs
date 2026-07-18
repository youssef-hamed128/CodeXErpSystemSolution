using Microsoft.AspNetCore.Mvc;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Warehouses;

namespace CodeXErpSystem.Controllers
{
    public class StockAdjustmentController : Controller
    {
        private readonly IStockAdjustmentService _stockAdjustmentService;

        public StockAdjustmentController(IStockAdjustmentService stockAdjustmentService)
        {
            _stockAdjustmentService = stockAdjustmentService;
        }

        public IActionResult Index()
        {
            var data = _stockAdjustmentService.GetStockAdjustmentInitialData();
            return View(data);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create([FromBody] StockAdjustmentViewModel model)
        {
            await _stockAdjustmentService.CreateAsync(model);
            return Json(new { success = true, message = "Stock Adjustment created successfully." });
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Edit([FromBody] StockAdjustmentViewModel model)
        {
            await _stockAdjustmentService.UpdateAsync(model);
            return Json(new { success = true, message = "Stock Adjustment updated successfully." });
        }

        [HttpPost]
        public async    Task<IActionResult> Delete(int id)
        {
            await _stockAdjustmentService.DeleteAsync(id);
            return Json(new { success = true, message = "Stock Adjustment deleted successfully." });
        }
    }
}
