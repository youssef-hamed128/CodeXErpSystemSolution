using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "مدير النظام, مبيعات")]
    public class SalesInvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalesInvoiceController(IInvoiceService invoiceService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Type == InvoiceType.Sales, includeProperties: "Customer", orderBy: q => q.OrderByDescending(x => x.Id));
            
            return View(_mapper.Map<IEnumerable<InvoiceViewModel>>(invoices));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownsAsync();
            var model = new InvoiceCreateViewModel { Type = InvoiceType.Sales, Date = DateTime.UtcNow };
            model.InvoiceNumber = await _invoiceService.GenerateInvoiceNumberAsync(InvoiceType.Sales);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateViewModel model)
        {
            model.Type = InvoiceType.Sales;

            if (ModelState.IsValid)
            {
                try
                {
                    string currentUserId = "Admin"; 
                    var result = await _invoiceService.CreateInvoiceAsync(model, currentUserId);
                    
                    TempData["Success"] = $"تم حفظ الفاتورة بنجاح. رقم الفاتورة: {result.InvoiceNumber}";
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

        public async Task<IActionResult> Print(int id)
        {
            var invoice = (await _unitOfWork.GetRepository<Invoice>().FindAsync(i => i.Id == id, includeProperties: "Customer,Items.Product")).FirstOrDefault();
            if (invoice == null || invoice.Type != InvoiceType.Sales)
            {
                return NotFound();
            }

            var company = (await _unitOfWork.GetRepository<CompanySettings>().FindAsync()).FirstOrDefault();
            ViewBag.Company = company;

            var model = _mapper.Map<InvoiceViewModel>(invoice);
            return View(model);
        }

        private async Task PrepareDropdownsAsync()
        {
            var customers = await _unitOfWork.GetRepository<Customer>().FindAsync();
            var warehouses = await _unitOfWork.GetRepository<Warehouse>().FindAsync();
            var products = await _unitOfWork.GetRepository<Product>().FindAsync();
            var categories = await _unitOfWork.GetRepository<ProductCategory>().FindAsync();

            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.ProductsList = products;
        }
    }
}




