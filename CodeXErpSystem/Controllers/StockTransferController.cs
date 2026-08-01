using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeXErpSystem.BLL.Services.Interfaces;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, مشتريات ومخازن")]
    public class StockTransferController : Controller
    {
        private readonly IStockTransferService _stockTransferService;

        public StockTransferController(IStockTransferService stockTransferService)
        {
            _stockTransferService = stockTransferService;
        }

        public IActionResult Index()
        {
            var data = _stockTransferService.GetStockTransferInitialData();
            return View(data);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create([FromBody] CodeXErpSystem.BLL.ViewModels.Warehouses.StockTransferViewModel model)
        {
            try
            {
                await _stockTransferService.CreateAsync(model);
                return Json(new { success = true, message = "تم تحويل المخزون بين المستودعات بنجاح." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Edit([FromBody] CodeXErpSystem.BLL.ViewModels.Warehouses.StockTransferViewModel model)
        {
            await _stockTransferService.UpdateAsync(model);
            return Json(new { success = true, message = "Stock Transfer updated successfully." });
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Delete(int id)
        {
            await _stockTransferService.DeleteAsync(id);
            return Json(new { success = true, message = "Stock Transfer deleted successfully." });
        }
    }
}

