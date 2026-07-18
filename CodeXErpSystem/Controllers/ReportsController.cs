using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BalanceSheet()
        {
            return View();
        }

        public IActionResult InventoryReport()
        {
            return View();
        }

        [ActionName("View")]
        public IActionResult ReportView(string id)
        {
            return View("View");
        }
    }
}
