using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام")]
    public class SettingsController : Controller
    {
        private readonly CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork _unitOfWork;
        private readonly AutoMapper.IMapper _mapper;

        public SettingsController(CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var repo = _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.CompanySettings>();
            var settingsList = await repo.FindAsync();
            var settings = settingsList.FirstOrDefault() ?? new CodeXErpSystem.DAL.Entites.CompanySettings();

            var model = _mapper.Map<CodeXErpSystem.BLL.ViewModels.Settings.CompanySettingsViewModel>(settings);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CodeXErpSystem.BLL.ViewModels.Settings.CompanySettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.CompanySettings>();
                var settingsList = await repo.FindAsync();
                var settings = settingsList.FirstOrDefault();

                if (settings == null)
                {
                    settings = _mapper.Map<CodeXErpSystem.DAL.Entites.CompanySettings>(model);
                    repo.Add(settings);
                }
                else
                {
                    // Update existing
                    _mapper.Map(model, settings);
                    repo.Update(settings);
                }

                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "تم حفظ الإعدادات بنجاح";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}

