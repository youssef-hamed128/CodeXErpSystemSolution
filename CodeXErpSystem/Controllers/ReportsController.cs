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

        [ActionName("View")]
        public IActionResult ReportView(string id)
        {
            if (id == "income_statement") return RedirectToAction("IncomeStatement");
            if (id == "trial_balance") return RedirectToAction("TrialBalance");
            if (id == "low_stock") return RedirectToAction("LowStock");
            if (id == "sales_by_customer") return RedirectToAction("SalesByCustomer");
            if (id == "sales_by_product") return RedirectToAction("SalesByProduct");
            if (id == "zatca_tax") return RedirectToAction("ZatcaTax");

            return View("View");
        }
    }
}

