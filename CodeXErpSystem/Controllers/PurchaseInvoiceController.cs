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
    [Authorize(Roles = "مدير النظام, مشتريات ومخازن")]
    public class PurchaseInvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseInvoiceController(IInvoiceService invoiceService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Type == InvoiceType.Purchase, includeProperties: "Supplier", orderBy: q => q.OrderByDescending(x => x.Id));

            var returnInvoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Type == InvoiceType.PurchaseReturn && !string.IsNullOrEmpty(i.ReferenceNumber));

            var returnGrouped = returnInvoices.Where(r => r.ReferenceNumber != null)
                                              .GroupBy(r => r.ReferenceNumber!)
                                              .ToDictionary(g => g.Key, g => g.Count());

            var models = _mapper.Map<IEnumerable<InvoiceViewModel>>(invoices).ToList();
            foreach (var model in models)
            {
                if (!string.IsNullOrEmpty(model.InvoiceNumber) && returnGrouped.ContainsKey(model.InvoiceNumber))
                {
                    model.HasReturn = true;
                    model.ReturnCount = returnGrouped[model.InvoiceNumber];
                }
            }

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var cashSupplier = (await _unitOfWork.GetRepository<Supplier>().FindAsync(s => s.Name == "مورد نقدي" && !s.IsDeleted)).FirstOrDefault();
            if (cashSupplier == null)
            {
                cashSupplier = new Supplier
                {
                    Name = "مورد نقدي",
                    Phone = "-",
                    Email = "cash@supplier.com",
                    Address = "مورد نقدي",
                    Balance = 0,
                    CreatedBy = "System"
                };
                _unitOfWork.GetRepository<Supplier>().Add(cashSupplier);
                await _unitOfWork.CompleteAsync();
            }

            await PrepareDropdownsAsync();
            var model = new InvoiceCreateViewModel { Type = InvoiceType.Purchase, Date = DateTime.UtcNow };
            model.InvoiceNumber = await _invoiceService.GenerateInvoiceNumberAsync(InvoiceType.Purchase);
            var mainWarehouse = (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => w.Name == "المخزن الرئيسي" && !w.IsDeleted)).FirstOrDefault()
                                ?? (await _unitOfWork.GetRepository<Warehouse>().FindAsync(w => !w.IsDeleted)).FirstOrDefault();
            if (mainWarehouse != null)
            {
                model.WarehouseId = mainWarehouse.Id;
            }
            if (cashSupplier != null)
            {
                model.SupplierId = cashSupplier.Id;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateViewModel model)
        {
            model.Type = InvoiceType.Purchase;

            // تعيين تاريخ استحقاق افتراضي إذا كان فارغاً
            if (model.DueDate == default) model.DueDate = model.Date == default ? DateTime.UtcNow : model.Date;
            if (model.Date == default) model.Date = DateTime.UtcNow;

            // إزالة أخطاء الحقول الغير مطلوبة
            ModelState.Remove("InvoiceNumber");
            ModelState.Remove("AttachmentUrl");
            ModelState.Remove("Notes");
            ModelState.Remove("PaidAmount");
            ModelState.Remove("Type");
            ModelState.Remove("CustomerId");
            ModelState.Remove("SupplierId");
            ModelState.Remove("ReferenceNumber");
            ModelState.Remove("Status");
            ModelState.Remove("PaymentMethod");

            var keysToRemove = ModelState.Keys.Where(k => k.Contains("SalePrice") || k.Contains("UnitPrice") || k.Contains("Quantity") || k.Contains("DiscountAmount") || k.Contains("DiscountPercentage") || k.Contains("TaxPercentage")).ToList();
            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

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

        public IActionResult Details(int id)
        {
            return RedirectToAction(nameof(Print), new { id });
        }

        public async Task<IActionResult> Print(int id)
        {
            var invoice = (await _unitOfWork.GetRepository<Invoice>().FindAsync(i => i.Id == id, includeProperties: "Supplier,Items.Product")).FirstOrDefault();
            if (invoice == null || invoice.Type != InvoiceType.Purchase)
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
            var suppliers = await _unitOfWork.GetRepository<Supplier>().FindAsync();
            var warehouses = await _unitOfWork.GetRepository<Warehouse>().FindAsync();
            var products = await _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "StockQuantities");
            var categories = await _unitOfWork.GetRepository<ProductCategory>().FindAsync();

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name");
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.ProductsList = products.Select(p => new {
                p.Id,
                p.Name,
                p.CategoryId,
                p.SalePrice,
                p.PurchasePrice,
                p.UnitOfMeasure,
                AvailableQty = p.StockQuantities != null ? p.StockQuantities.Sum(s => s.Quantity) : 0,
                StockByWarehouse = p.StockQuantities != null ? p.StockQuantities.Select(sq => new { sq.WarehouseId, sq.Quantity }).ToList() : null
            }).ToList();
        }
    }
}



