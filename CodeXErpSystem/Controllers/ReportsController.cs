using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using CodeXErpSystem.BLL.ViewModels.Reports;
using CodeXErpSystem.DAL.Entites;
using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CodeXErpSystem.BLL.ViewModels.Accounting;

namespace CodeXErpSystem.Controllers
{
    [Authorize(Roles = "مدير النظام, مبيعات, مشتريات ومخازن, محاسب")]
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> BalanceSheet(DateTime? asOfDate)
        {
            DateTime date = asOfDate ?? DateTime.Today;

            var accounts = await _unitOfWork.GetRepository<Account>().FindAsync(includeProperties: "JournalEntryLines");
            var accountVms = new List<AccountViewModel>();

            decimal totalAssets = 0;
            decimal totalLiabilities = 0;
            decimal totalEquity = 0;

            foreach (var account in accounts)
            {
                // Calculate balance up to date
                decimal balance = account.JournalEntryLines
                    .Where(l => l.JournalEntry != null && l.JournalEntry.Date <= date && l.JournalEntry.Status == CodeXErpSystem.DAL.Entites.Enums.JournalEntryStatus.Posted)
                    .Sum(l => l.Debit - l.Credit);

                // Or if they don't use journal entries strictly yet, just use account.Balance as a fallback
                if (!account.JournalEntryLines.Any())
                {
                    balance = account.Balance; // Fallback
                }

                // Normal balance logic
                if (account.Code.StartsWith("1")) // Assets (Debit normal)
                {
                    totalAssets += balance;
                }
                else if (account.Code.StartsWith("2")) // Liabilities (Credit normal)
                {
                    balance = -balance; // Reverse sign for display
                    totalLiabilities += balance;
                }
                else if (account.Code.StartsWith("3")) // Equity (Credit normal)
                {
                    balance = -balance;
                    totalEquity += balance;
                }
                else if (account.Code.StartsWith("4")) // Revenue (Credit normal, rolls into Equity)
                {
                    balance = -balance;
                    totalEquity += balance;
                    continue; // Optional: include in Retained Earnings row later, skip displaying individual revenue in balance sheet
                }
                else if (account.Code.StartsWith("5")) // Expenses (Debit normal, reduces Equity)
                {
                    balance = -balance; // Expense reduces equity (Credit - Debit)
                    totalEquity += balance;
                    continue; // Skip individual display
                }

                if (balance != 0 || account.Code.StartsWith("1") || account.Code.StartsWith("2") || account.Code.StartsWith("3"))
                {
                    accountVms.Add(new AccountViewModel
                    {
                        Id = account.Id,
                        Code = account.Code,
                        Name = account.Name,
                        Balance = balance
                    });
                }
            }

            var model = new BalanceSheetViewModel
            {
                AsOfDate = date,
                TotalAssets = totalAssets,
                TotalLiabilities = totalLiabilities,
                TotalEquity = totalEquity,
                Accounts = accountVms.OrderBy(a => a.Code).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> IncomeStatement(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Today.Year, 1, 1);
            var end = endDate ?? DateTime.Today;

            var accounts = await _unitOfWork.GetRepository<Account>().FindAsync(includeProperties: "JournalEntryLines");
            
            var model = new IncomeStatementViewModel
            {
                StartDate = start,
                EndDate = end
            };

            foreach (var account in accounts)
            {
                if (!account.Code.StartsWith("4") && !account.Code.StartsWith("5"))
                    continue;

                decimal balance = account.JournalEntryLines
                    .Where(l => l.JournalEntry != null && l.JournalEntry.Date >= start && l.JournalEntry.Date <= end && l.JournalEntry.Status == CodeXErpSystem.DAL.Entites.Enums.JournalEntryStatus.Posted)
                    .Sum(l => l.Credit - l.Debit); // For income statement: Credit is positive for Revenue. Expenses will be negative initially.

                if (!account.JournalEntryLines.Any())
                {
                    // Fallback
                    balance = account.Code.StartsWith("4") ? account.Balance : -account.Balance;
                }

                if (account.Code.StartsWith("4") && balance != 0)
                {
                    model.RevenueAccounts.Add(new AccountViewModel { Code = account.Code, Name = account.Name, Balance = balance });
                    model.TotalRevenues += balance;
                }
                else if (account.Code.StartsWith("5") && balance != 0)
                {
                    // Expense is displayed as positive number
                    var expBalance = -balance; 
                    model.ExpenseAccounts.Add(new AccountViewModel { Code = account.Code, Name = account.Name, Balance = expBalance });
                    model.TotalExpenses += expBalance;
                }
            }

            model.RevenueAccounts = model.RevenueAccounts.OrderBy(a => a.Code).ToList();
            model.ExpenseAccounts = model.ExpenseAccounts.OrderBy(a => a.Code).ToList();

            return View(model);
        }

        public async Task<IActionResult> TrialBalance(DateTime? asOfDate)
        {
            var date = asOfDate ?? DateTime.Today;
            var accounts = await _unitOfWork.GetRepository<Account>().FindAsync(includeProperties: "JournalEntryLines");
            
            var model = new TrialBalanceViewModel { AsOfDate = date };

            foreach (var account in accounts)
            {
                decimal balance = account.JournalEntryLines
                    .Where(l => l.JournalEntry != null && l.JournalEntry.Date <= date && l.JournalEntry.Status == CodeXErpSystem.DAL.Entites.Enums.JournalEntryStatus.Posted)
                    .Sum(l => l.Debit - l.Credit);

                if (!account.JournalEntryLines.Any())
                {
                    balance = account.Balance;
                    if (account.Code.StartsWith("2") || account.Code.StartsWith("3") || account.Code.StartsWith("4"))
                        balance = -balance; // convert absolute balance to Debit/Credit sign (Debit is positive, Credit is negative)
                }

                if (balance != 0)
                {
                    var item = new TrialBalanceItem { Code = account.Code, Name = account.Name };
                    if (balance > 0)
                    {
                        item.Debit = balance;
                        model.TotalDebit += balance;
                    }
                    else
                    {
                        item.Credit = -balance;
                        model.TotalCredit += -balance;
                    }
                    model.Accounts.Add(item);
                }
            }

            model.Accounts = model.Accounts.OrderBy(a => a.Code).ToList();
            return View(model);
        }

        public async Task<IActionResult> InventoryReport()
        {
            var products = await _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "StockQuantities");
            var model = new InventoryReportViewModel();

            foreach (var p in products)
            {
                var totalQty = p.StockQuantities.Sum(s => s.Quantity);
                if (totalQty > 0)
                {
                    var item = new InventoryItemReport
                    {
                        Code = p.Code,
                        Name = p.Name,
                        Quantity = totalQty,
                        UnitCost = p.PurchasePrice ?? 0
                    };
                    model.Items.Add(item);
                    model.TotalInventoryValue += item.TotalValue;
                }
            }

            model.Items = model.Items.OrderByDescending(i => i.TotalValue).ToList();
            return View(model);
        }

        public async Task<IActionResult> LowStock()
        {
            var products = await _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "StockQuantities");
            var items = new List<InventoryItemReport>();

            foreach (var p in products)
            {
                var totalQty = p.StockQuantities.Sum(s => s.Quantity);
                if (totalQty <= p.MinStockLevel)
                {
                    items.Add(new InventoryItemReport
                    {
                        Code = p.Code,
                        Name = p.Name,
                        Quantity = totalQty,
                        UnitCost = p.PurchasePrice ?? 0
                    });
                }
            }

            return View(items.OrderBy(i => i.Quantity).ToList());
        }

        public async Task<IActionResult> SalesByCustomer(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var end = endDate ?? DateTime.Today;

            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Sales && i.Date >= start && i.Date <= end,
                includeProperties: "Customer");

            var model = new SalesByCustomerViewModel { StartDate = start, EndDate = end };

            var grouped = invoices.Where(i => i.Customer != null).GroupBy(i => i.Customer!.Name);
            foreach (var g in grouped)
            {
                model.Customers.Add(new CustomerSalesItem
                {
                    CustomerName = g.Key,
                    InvoicesCount = g.Count(),
                    TotalSales = g.Sum(i => i.TotalAmount)
                });
            }

            model.Customers = model.Customers.OrderByDescending(c => c.TotalSales).ToList();
            return View(model);
        }

        public async Task<IActionResult> SalesByProduct(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var end = endDate ?? DateTime.Today;

            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Sales && i.Date >= start && i.Date <= end,
                includeProperties: "Items.Product");

            var model = new SalesByProductViewModel { StartDate = start, EndDate = end };
            var productSales = new Dictionary<int, ProductSalesItem>();

            foreach (var inv in invoices)
            {
                foreach (var item in inv.Items)
                {
                    if (item.Product == null) continue;

                    if (!productSales.ContainsKey(item.ProductId))
                    {
                        productSales[item.ProductId] = new ProductSalesItem
                        {
                            Code = item.Product.Code,
                            ProductName = item.Product.Name,
                            QuantitySold = 0,
                            TotalRevenue = 0
                        };
                    }

                    productSales[item.ProductId].QuantitySold += item.Quantity;
                    productSales[item.ProductId].TotalRevenue += item.Total;
                }
            }

            model.Products = productSales.Values.OrderByDescending(p => p.TotalRevenue).ToList();
            return View(model);
        }

        public async Task<IActionResult> ZatcaTax(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var end = endDate ?? DateTime.Today;

            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.Date >= start && i.Date <= end);

            var model = new ZatcaTaxViewModel { StartDate = start, EndDate = end };

            foreach (var inv in invoices)
            {
                if (inv.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Sales)
                {
                    model.TotalSales += inv.SubTotal;
                    model.OutputTax += inv.TaxAmount;
                }
                else if (inv.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Purchase)
                {
                    model.TotalPurchases += inv.SubTotal;
                    model.InputTax += inv.TaxAmount;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> CustomerStatement(int? customerId, DateTime? startDate, DateTime? endDate)
        {
            var customers = await _unitOfWork.GetRepository<Customer>().FindAsync(c => !c.IsDeleted);
            ViewBag.Customers = customers.OrderBy(c => c.Name).Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = customerId.HasValue && c.Id == customerId.Value
            }).ToList();

            var model = new CustomerStatementViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };

            if (!customerId.HasValue || customerId.Value <= 0)
                return View(model);

            var selectedCustomerId = customerId.Value;

            var customer = await _unitOfWork.GetRepository<Customer>().GetById(selectedCustomerId);
            if (customer == null)
                return View(model);

            model.CustomerId = customer.Id;
            model.CustomerName = customer.Name;
            model.Phone = customer.Phone;
            model.Address = customer.Address;
            model.TaxNumber = customer.TaxNumber;
            model.CustomerCurrentBalance = customer.Balance ?? 0;

            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.CustomerId == selectedCustomerId && i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Sales,
                includeProperties: "Payments");

            if (startDate.HasValue)
                invoices = invoices.Where(i => i.Date >= startDate.Value);
            if (endDate.HasValue)
                invoices = invoices.Where(i => i.Date <= endDate.Value);

            foreach (var inv in invoices.OrderBy(i => i.Date))
            {
                decimal paid = 0;
                if (inv.Status == CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Paid || inv.PaidAmount >= inv.TotalAmount - 0.01m)
                {
                    paid = inv.TotalAmount;
                }
                else if (inv.PaidAmount > 0)
                {
                    paid = inv.PaidAmount;
                }
                else if (inv.Payments != null && inv.Payments.Any())
                {
                    paid = inv.Payments.Sum(p => p.Amount);
                }

                var rem = inv.TotalAmount - paid;
                string statusText = "غير مسدد";
                if (rem <= 0.01m) statusText = "مدفوع بالكامل";
                else if (paid > 0.01m) statusText = "سداد جزئي";

                model.Invoices.Add(new StatementInvoiceItem
                {
                    InvoiceId = inv.Id,
                    InvoiceNumber = inv.InvoiceNumber,
                    Date = inv.Date,
                    TotalAmount = inv.TotalAmount,
                    PaidAmount = paid,
                    RemainingAmount = rem > 0 ? rem : 0,
                    Status = statusText,
                    Note = inv.Note
                });
            }

            var receipts = await _unitOfWork.GetRepository<Payment>().FindAsync(
                p => p.CustomerId == selectedCustomerId,
                includeProperties: "Invoice");

            if (startDate.HasValue)
                receipts = receipts.Where(p => p.Date >= startDate.Value);
            if (endDate.HasValue)
                receipts = receipts.Where(p => p.Date <= endDate.Value);

            foreach (var rec in receipts.OrderBy(r => r.Date))
            {
                string methodText = rec.PaymentMethod == CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Cash ? "نقدي" :
                                    rec.PaymentMethod == CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.BankTransfer ? "تحويل بنكي" :
                                    rec.PaymentMethod == CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Check ? "شيك" : "أخرى";

                model.Receipts.Add(new StatementPaymentItem
                {
                    PaymentId = rec.Id,
                    ReceiptNumber = rec.ReceiptNumber,
                    Date = rec.Date,
                    Amount = rec.Amount,
                    PaymentMethodName = methodText,
                    LinkedInvoiceNumber = rec.Invoice != null ? rec.Invoice.InvoiceNumber : "دفعة عامة",
                    Reference = rec.Reference
                });
            }

            model.TotalInvoicesAmount = model.Invoices.Sum(i => i.TotalAmount);
            model.TotalPaidAmount = model.Invoices.Sum(i => i.PaidAmount);
            model.TotalRemainingAmount = model.Invoices.Sum(i => i.RemainingAmount);

            return View(model);
        }

        public async Task<IActionResult> SupplierStatement(int? supplierId, DateTime? startDate, DateTime? endDate)
        {
            var suppliers = await _unitOfWork.GetRepository<Supplier>().FindAsync(s => !s.IsDeleted);
            ViewBag.Suppliers = suppliers.OrderBy(s => s.Name).Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = supplierId.HasValue && s.Id == supplierId.Value
            }).ToList();

            var model = new SupplierStatementViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };

            if (!supplierId.HasValue || supplierId.Value <= 0)
                return View(model);

            var selectedSupplierId = supplierId.Value;

            var supplier = await _unitOfWork.GetRepository<Supplier>().GetById(selectedSupplierId);
            if (supplier == null)
                return View(model);

            model.SupplierId = supplier.Id;
            model.SupplierName = supplier.Name;
            model.Phone = supplier.Phone;
            model.Address = supplier.Address;
            model.TaxNumber = supplier.TaxNumber;
            model.SupplierCurrentBalance = supplier.Balance ?? 0;

            var invoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                i => i.SupplierId == selectedSupplierId && i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Purchase,
                includeProperties: "Payments");

            if (startDate.HasValue)
                invoices = invoices.Where(i => i.Date >= startDate.Value);
            if (endDate.HasValue)
                invoices = invoices.Where(i => i.Date <= endDate.Value);

            foreach (var inv in invoices.OrderBy(i => i.Date))
            {
                decimal paid = 0;
                if (inv.Status == CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Paid || inv.PaidAmount >= inv.TotalAmount - 0.01m)
                {
                    paid = inv.TotalAmount;
                }
                else if (inv.PaidAmount > 0)
                {
                    paid = inv.PaidAmount;
                }
                else if (inv.Payments != null && inv.Payments.Any())
                {
                    paid = inv.Payments.Sum(p => p.Amount);
                }

                var rem = inv.TotalAmount - paid;
                string statusText = "غير مسدد";
                if (rem <= 0.01m) statusText = "مدفوع بالكامل";
                else if (paid > 0.01m) statusText = "سداد جزئي";

                model.Invoices.Add(new StatementInvoiceItem
                {
                    InvoiceId = inv.Id,
                    InvoiceNumber = inv.InvoiceNumber,
                    Date = inv.Date,
                    TotalAmount = inv.TotalAmount,
                    PaidAmount = paid,
                    RemainingAmount = rem > 0 ? rem : 0,
                    Status = statusText,
                    Note = inv.Note
                });
            }

            var payments = await _unitOfWork.GetRepository<Payment>().FindAsync(
                p => p.SupplierId == selectedSupplierId,
                includeProperties: "Invoice");

            if (startDate.HasValue)
                payments = payments.Where(p => p.Date >= startDate.Value);
            if (endDate.HasValue)
                payments = payments.Where(p => p.Date <= endDate.Value);

            foreach (var pay in payments.OrderBy(p => p.Date))
            {
                string methodText = pay.PaymentMethod == CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Cash ? "نقدي" :
                                    pay.PaymentMethod == CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.BankTransfer ? "تحويل بنكي" :
                                    pay.PaymentMethod == CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Check ? "شيك" : "أخرى";

                model.Payments.Add(new StatementPaymentItem
                {
                    PaymentId = pay.Id,
                    ReceiptNumber = pay.ReceiptNumber,
                    Date = pay.Date,
                    Amount = pay.Amount,
                    PaymentMethodName = methodText,
                    LinkedInvoiceNumber = pay.Invoice != null ? pay.Invoice.InvoiceNumber : "دفعة عامة",
                    Reference = pay.Reference
                });
            }

            model.TotalInvoicesAmount = model.Invoices.Sum(i => i.TotalAmount);
            model.TotalPaidAmount = model.Invoices.Sum(i => i.PaidAmount);
            model.TotalRemainingAmount = model.Invoices.Sum(i => i.RemainingAmount);

            return View(model);
        }

        public async Task<IActionResult> DebtsReport()
        {
            var customers = await _unitOfWork.GetRepository<Customer>().FindAsync(includeProperties: "Invoices,Payments");
            var suppliers = await _unitOfWork.GetRepository<Supplier>().FindAsync(includeProperties: "Invoices,Payments");

            var model = new DebtsReportViewModel();

            foreach (var cust in customers)
            {
                var validInvoices = cust.Invoices.Where(i => i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Sales).ToList();
                decimal totalInv = validInvoices.Sum(i => i.TotalAmount);
                decimal totalPaid = validInvoices.Sum(i => i.PaidAmount);
                decimal rem = totalInv - totalPaid;

                if (cust.Balance != rem)
                {
                    cust.Balance = rem;
                    _unitOfWork.GetRepository<Customer>().Update(cust);
                }

                string status = "خالص (بدون مديونية)";
                if (rem > 0.01m) status = "مدين لنا";
                else if (rem < -0.01m) status = "رصيد دائن للعميل";

                model.Customers.Add(new CustomerDebtItem
                {
                    CustomerId = cust.Id,
                    CustomerName = cust.Name,
                    Phone = cust.Phone,
                    TotalInvoices = totalInv,
                    TotalPaid = totalPaid,
                    RemainingBalance = rem,
                    Status = status
                });
            }

            foreach (var sup in suppliers)
            {
                var validInvoices = sup.Invoices.Where(i => i.Type == CodeXErpSystem.DAL.Entites.Enums.InvoiceType.Purchase).ToList();
                decimal totalInv = validInvoices.Sum(i => i.TotalAmount);
                decimal totalPaid = validInvoices.Sum(i => i.PaidAmount);
                decimal rem = totalInv - totalPaid;

                if (sup.Balance != rem)
                {
                    sup.Balance = rem;
                    _unitOfWork.GetRepository<Supplier>().Update(sup);
                }

                string status = "خالص (بدون مستحقات)";
                if (rem > 0.01m) status = "مستحق له (دائن)";
                else if (rem < -0.01m) status = "رصيد مدين للمورد";

                model.Suppliers.Add(new SupplierDebtItem
                {
                    SupplierId = sup.Id,
                    SupplierName = sup.Name,
                    Phone = sup.Phone,
                    TotalInvoices = totalInv,
                    TotalPaid = totalPaid,
                    RemainingBalance = rem,
                    Status = status
                });
            }

            await _unitOfWork.CompleteAsync();

            model.TotalCustomerDebts = model.Customers.Where(c => c.RemainingBalance > 0).Sum(c => c.RemainingBalance);
            model.TotalSupplierPayables = model.Suppliers.Where(s => s.RemainingBalance > 0).Sum(s => s.RemainingBalance);

            return View(model);
        }

        [ActionName("View")]
        public IActionResult ReportView(string id)
        {
            if (id == "debts_report") return RedirectToAction("DebtsReport");
            if (id == "income_statement") return RedirectToAction("IncomeStatement");
            if (id == "trial_balance") return RedirectToAction("TrialBalance");
            if (id == "low_stock") return RedirectToAction("LowStock");
            if (id == "sales_by_customer") return RedirectToAction("SalesByCustomer");
            if (id == "sales_by_product") return RedirectToAction("SalesByProduct");
            if (id == "zatca_tax") return RedirectToAction("ZatcaTax");
            if (id == "customer_statement") return RedirectToAction("CustomerStatement");
            if (id == "supplier_statement") return RedirectToAction("SupplierStatement");

            return View("View");
        }
    }
}

