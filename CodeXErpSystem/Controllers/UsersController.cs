using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork _unitOfWork;
        private readonly AutoMapper.IMapper _mapper;

        public UsersController(CodeXErpSystem.DAL.Repository.Inetrfaces.IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var repo = _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.ApplicationUser>();
            var users = await repo.FindAsync(null, "Role");
            
            var roles = await _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.Role>().FindAsync();
            ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roles, "Id", "Name");

            var model = _mapper.Map<IEnumerable<CodeXErpSystem.BLL.ViewModels.Settings.UserViewModel>>(users);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CodeXErpSystem.BLL.ViewModels.Settings.UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.ApplicationUser>();
                var user = _mapper.Map<CodeXErpSystem.DAL.Entites.ApplicationUser>(model);
                
                // Simple Hash for demo (In production use proper hashing)
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                    user.PasswordHash = Convert.ToBase64String(bytes);
                }
                
                repo.Add(user);
                await _unitOfWork.CompleteAsync();
                
                TempData["Success"] = "تمت إضافة المستخدم بنجاح";
            }
            else
            {
                TempData["Error"] = "يرجى التحقق من البيانات المدخلة";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var repo = _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.ApplicationUser>();
            repo.Delete(id);
            await _unitOfWork.CompleteAsync();
            return Json(new { success = true, message = "تم حذف المستخدم بنجاح" });
        }
    }
}
