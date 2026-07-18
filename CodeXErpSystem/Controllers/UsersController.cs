using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
