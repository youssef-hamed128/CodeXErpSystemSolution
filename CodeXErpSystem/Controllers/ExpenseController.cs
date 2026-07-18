using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels.Expenses;

namespace CodeXErpSystem.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _expenseService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _expenseService.CreateAsync(model);
                return Json(new { success = true, message = "Expense created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _expenseService.UpdateAsync(model);
                return Json(new { success = true, message = "Expense updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _expenseService.DeleteAsync(id);
            return Json(new { success = true, message = "Expense deleted successfully" });
        }
    }
}
