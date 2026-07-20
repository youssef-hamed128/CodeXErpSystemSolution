using Microsoft.AspNetCore.Authorization;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels.Accounting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, محاسب")]
    public class JournalEntryController : Controller
    {
        private readonly IJournalEntryService _journalEntryService;

        public JournalEntryController(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _journalEntryService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(JournalEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _journalEntryService.CreateAsync(model);
                return Json(new { success = true, message = "Journal Entry created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(JournalEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _journalEntryService.UpdateAsync(model);
                return Json(new { success = true, message = "Journal Entry updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _journalEntryService.DeleteAsync(id);
            return Json(new { success = true, message = "Journal Entry deleted successfully" });
        }
    }
}

