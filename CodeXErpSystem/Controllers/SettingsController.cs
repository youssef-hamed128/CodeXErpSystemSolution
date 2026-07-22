using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام")]
    public class SettingsController : Controller
    {
        private readonly CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork _unitOfWork;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public SettingsController(CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork unitOfWork, AutoMapper.IMapper mapper, IWebHostEnvironment env, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
            _config = config;
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

        [HttpPost]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                string backupsFolder = Path.Combine(_env.WebRootPath, "backups");
                if (!Directory.Exists(backupsFolder))
                {
                    Directory.CreateDirectory(backupsFolder);
                }

                string fileName = $"CodeXERP_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string filePath = Path.Combine(backupsFolder, fileName);

                string connStr = _config.GetConnectionString("DefaultConnection") ?? "";
                
                // Using raw ADO.NET to avoid EF Core timeout on large backups
                using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"BACKUP DATABASE [CodeXERP] TO DISK = '{filePath}' WITH FORMAT, INIT, COMPRESSION;";
                        cmd.CommandTimeout = 300; // 5 minutes
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                // Optionally delete the file from server after reading into memory
                System.IO.File.Delete(filePath);

                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء النسخ الاحتياطي: " + ex.Message });
            }
        }
    }
}

