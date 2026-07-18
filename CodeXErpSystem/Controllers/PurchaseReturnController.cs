using AutoMapper;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Invoice;
using CodeXErpSystem.DAL.Entites.Enums;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseReturnController(IInvoiceService invoiceService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Type == InvoiceType.PurchaseReturn, includeProperties: "Supplier", orderBy: q => q.OrderByDescending(x => x.Id));
            
            return View(_mapper.Map<IEnumerable<InvoiceViewModel>>(invoices));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownsAsync();
            var model = new InvoiceCreateViewModel { Type = InvoiceType.PurchaseReturn, Date = DateTime.UtcNow };
            model.InvoiceNumber = await _invoiceService.GenerateInvoiceNumberAsync(InvoiceType.PurchaseReturn);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateViewModel model)
        {
            model.Type = InvoiceType.PurchaseReturn;

            if (ModelState.IsValid)
            {
                try
                {
                    string currentUserId = "Admin"; 
                    var result = await _invoiceService.CreateInvoiceAsync(model, currentUserId);
                    
                    TempData["Success"] = $"تم حفظ مرتجع المشتريات بنجاح. رقم الفاتورة: {result.InvoiceNumber}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "حدث خطأ أثناء الحفظ: " + ex.Message);
                }
            }
            
            await PrepareDropdownsAsync();
            return View(model);
        }

        private async Task PrepareDropdownsAsync()
        {
            var suppliers = await _unitOfWork.GetRepository<Supplier>().FindAsync();
            var warehouses = await _unitOfWork.GetRepository<Warehouse>().FindAsync();
            var products = await _unitOfWork.GetRepository<Product>().FindAsync();

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name");
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.ProductsList = products;
        }
    }
}

