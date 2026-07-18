using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
