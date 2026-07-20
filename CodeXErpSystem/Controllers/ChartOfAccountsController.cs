using Microsoft.AspNetCore.Authorization;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Accounting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, محاسب")]
    public class ChartOfAccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public ChartOfAccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _accountService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _accountService.CreateAsync(model);
                return Json(new { success = true, message = "Account created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _accountService.UpdateAsync(model);
                return Json(new { success = true, message = "Account updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _accountService.DeleteAsync(id);
            return Json(new { success = true, message = "Account deleted successfully" });
        }
    }
}

