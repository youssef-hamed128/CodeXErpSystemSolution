using Microsoft.AspNetCore.Mvc;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using System.Threading.Tasks;
using System.Linq;

namespace CodeXErpSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IProductService productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var productsList = _productService.GetAllProducts();
            var products = await _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.Product>().FindAsync(includeProperties: "StockQuantities");

            ViewBag.TotalProducts = products.Count();
            ViewBag.Available = products.Count(p => p.StockQuantities.Sum(sq => sq.Quantity) > p.MinStockLevel);
            ViewBag.LowStock = products.Count(p => p.StockQuantities.Sum(sq => sq.Quantity) > 0 && p.StockQuantities.Sum(sq => sq.Quantity) <= p.MinStockLevel);
            ViewBag.OutOfStock = products.Count(p => p.StockQuantities.Sum(sq => sq.Quantity) == 0);

            ViewBag.Categories = await _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.ProductCategory>().GetAll(false);
            return View(productsList);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            await _productService.CreateAsync(model);
            return Json(new { success = true, message = "تم الإضافة بنجاح" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CodeXErpSystem.DAL.Entites.ProductCategory category)
        {
            if (string.IsNullOrEmpty(category.Name))
                return Json(new { success = false, message = "اسم المجموعة مطلوب" });
                
            _unitOfWork.GetRepository<CodeXErpSystem.DAL.Entites.ProductCategory>().Add(category);
            await _unitOfWork.CompleteAsync();
            
            return Json(new { success = true, message = "تمت إضافة المجموعة بنجاح", id = category.Id, name = category.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            await _productService.UpdateAsync(model);
            return Json(new { success = true, message = "تم التعديل بنجاح" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Json(new { success = true, message = "تم الحذف بنجاح" });
        }
    }
}
