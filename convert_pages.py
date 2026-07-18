import os
import re

# Paths
base_dir = r"C:\Users\youss\source\repos\CodeXErpSystemSolution"
html_dir = os.path.join(base_dir, "CodeXErpSystem", "Views", "Accounting")
view_dir = os.path.join(base_dir, "CodeXErpSystem", "Views")
controllers_dir = os.path.join(base_dir, "CodeXErpSystem", "Controllers")
bll_dir = os.path.join(base_dir, "CodeXErpSystem.BLL")
vm_dir = os.path.join(bll_dir, "ViewModels")
svc_dir = os.path.join(bll_dir, "Services")
iface_dir = os.path.join(svc_dir, "Interfaces")

os.makedirs(vm_dir, exist_ok=True)
os.makedirs(svc_dir, exist_ok=True)
os.makedirs(iface_dir, exist_ok=True)

pages = [
    {
        "html_file": "Customers.html",
        "controller": "CustomerController",
        "view_folder": "Customer",
        "model": "Customer",
        "service": "CustomerService",
        "vm_props": [
            "public int Id { get; set; }",
            "public string Name { get; set; } = string.Empty;",
            "public string? Phone { get; set; }",
            "public string? TaxNumber { get; set; }",
            "public string? Address { get; set; }",
            "public decimal? Balance { get; set; }",
            "public decimal? CreditLimit { get; set; }",
            "public DateTime CreatedAt { get; set; }",
            "public DateTime? UpdatedAt { get; set; }",
            "public bool IsDeleted { get; set; }"
        ],
        "row_template": """            <tr>
              <td><div style="font-weight:600;color:var(--text-primary);">@item.Name</div><div style="font-size:var(--fs-xs);color:var(--text-muted);">الرقم الضريبي: @item.TaxNumber</div></td>
              <td dir="ltr" style="text-align:right;">@item.Phone</td><td>@item.Address</td><td class="numeric" style="color:var(--primary);font-weight:700;">@item.Balance?.ToString("N2")</td><td class="numeric">@item.CreditLimit?.ToString("N2")</td><td>@(item.UpdatedAt?.ToString("dd MMMM yyyy") ?? item.CreatedAt.ToString("dd MMMM yyyy"))</td><td><span class="badge @(item.IsDeleted ? "badge-danger" : "badge-success")">@(item.IsDeleted ? "متوقف" : "نشط")</span></td>
              <td class="table-actions"><button class="action-btn view" onclick="showPreview('معاينة السجل')"><i class="fas fa-eye"></i></button><button class="action-btn edit" onclick="openModal('addCustomerModal')"><i class="fas fa-edit"></i></button><button class="action-btn delete" onclick="deleteRow(this, 'العميل')"><i class="fas fa-trash"></i></button></td>
            </tr>"""
    },
    {
        "html_file": "Suppliers.html",
        "controller": "SupplierController",
        "view_folder": "Supplier",
        "model": "Supplier",
        "service": "SupplierService",
        "vm_props": [
            "public int Id { get; set; }",
            "public string Name { get; set; } = string.Empty;",
            "public string? Phone { get; set; }",
            "public string? Address { get; set; }",
            "public decimal? Balance { get; set; }",
            "public DateTime CreatedAt { get; set; }",
            "public DateTime? UpdatedAt { get; set; }",
            "public bool IsDeleted { get; set; }"
        ],
        "row_template": """            <tr>
              <td><div style="font-weight:600;color:var(--text-primary);">@item.Name</div></td>
              <td dir="ltr" style="text-align:right;">@item.Phone</td><td>@item.Address</td><td class="numeric" style="color:var(--danger);font-weight:700;">@item.Balance?.ToString("N2")</td><td>صافي 30 يوم</td><td>@(item.UpdatedAt?.ToString("dd MMMM yyyy") ?? item.CreatedAt.ToString("dd MMMM yyyy"))</td><td><span class="badge @(item.IsDeleted ? "badge-danger" : "badge-success")">@(item.IsDeleted ? "متوقف" : "نشط")</span></td>
              <td class="table-actions"><button class="action-btn view" onclick="showPreview('معاينة السجل')"><i class="fas fa-eye"></i></button><button class="action-btn edit" onclick="openModal('addSupplierModal')"><i class="fas fa-edit"></i></button><button class="action-btn delete" onclick="deleteRow(this, 'المورد')"><i class="fas fa-trash"></i></button></td>
            </tr>"""
    },
    {
        "html_file": "ChartOfAccounts.html",
        "controller": "ChartOfAccountsController",
        "view_folder": "ChartOfAccounts",
        "model": "Account",
        "service": "AccountService",
        "vm_props": [
            "public int Id { get; set; }",
            "public string Code { get; set; } = string.Empty;",
            "public string Name { get; set; } = string.Empty;",
            "public decimal Balance { get; set; }",
            "public bool IsDeleted { get; set; }"
        ],
        "row_template": """            <tr>
              <td><span style="font-family:monospace;background:var(--bg-secondary);padding:2px 6px;border-radius:4px;color:var(--text-secondary);">@item.Code</span></td>
              <td style="font-weight:600;color:var(--text-primary);">@item.Name</td>
              <td>أصول</td>
              <td>الميزانية العمومية</td>
              <td class="numeric" style="font-weight:700;">@item.Balance.ToString("N2")</td>
              <td><span class="badge @(item.IsDeleted ? "badge-danger" : "badge-success")">@(item.IsDeleted ? "غير نشط" : "نشط")</span></td>
              <td class="table-actions"><button class="action-btn edit" onclick="openModal('accountModal')"><i class="fas fa-edit"></i></button><button class="action-btn delete"><i class="fas fa-trash"></i></button></td>
            </tr>"""
    },
    {
        "html_file": "JournalEntries.html",
        "controller": "JournalEntryController",
        "view_folder": "JournalEntry",
        "model": "JournalEntry",
        "service": "JournalEntryService",
        "vm_props": [
            "public int Id { get; set; }",
            "public string EntryNumber { get; set; } = string.Empty;",
            "public DateTime Date { get; set; }",
            "public string? Description { get; set; }",
            "public int Status { get; set; }"
        ],
        "row_template": """            <tr>
              <td style="font-weight:600;color:var(--primary);">#@item.EntryNumber</td>
              <td>@item.Date.ToString("dd MMMM yyyy")</td>
              <td>@item.Description</td>
              <td class="numeric">0.00</td>
              <td class="numeric">0.00</td>
              <td><span class="badge @(item.Status == 0 ? "badge-warning" : "badge-success")">@(item.Status == 0 ? "مسودة" : "مرحل")</span></td>
              <td class="table-actions"><button class="action-btn view" onclick="showPreview('تفاصيل القيد')"><i class="fas fa-eye"></i></button><button class="action-btn print"><i class="fas fa-print"></i></button></td>
            </tr>"""
    },
    {
        "html_file": "Payments.html",
        "controller": "PaymentController",
        "view_folder": "Payment",
        "model": "Payment",
        "service": "PaymentService",
        "vm_props": [
            "public int Id { get; set; }",
            "public string ReceiptNumber { get; set; } = string.Empty;",
            "public DateTime Date { get; set; }",
            "public decimal Amount { get; set; }",
            "public int PaymentMethod { get; set; }",
            "public string? Reference { get; set; }"
        ],
        "row_template": """            <tr>
              <td style="font-weight:600;color:var(--primary);">#@item.ReceiptNumber</td>
              <td>@item.Date.ToString("dd MMMM yyyy")</td>
              <td>@item.Reference</td>
              <td>إيصال استلام</td>
              <td>@item.PaymentMethod</td>
              <td class="numeric" style="font-weight:700;color:var(--success);">@item.Amount.ToString("N2")</td>
              <td><span class="badge badge-success">مكتمل</span></td>
              <td class="table-actions"><button class="action-btn view" onclick="showPreview('تفاصيل الدفعة')"><i class="fas fa-eye"></i></button><button class="action-btn print"><i class="fas fa-print"></i></button></td>
            </tr>"""
    },
    {
        "html_file": "Expenses.html",
        "controller": "ExpenseController",
        "view_folder": "Expense",
        "model": "Expense",
        "service": "ExpenseService",
        "vm_props": [
            "public int Id { get; set; }",
            "public DateTime Date { get; set; }",
            "public decimal Amount { get; set; }",
            "public string Category { get; set; } = null!;",
            "public string? Description { get; set; }",
            "public string PaymentMethod { get; set; } = null!;"
        ],
        "row_template": """            <tr>
              <td>@item.Date.ToString("dd MMMM yyyy")</td>
              <td>@item.Category</td>
              <td>@item.Description</td>
              <td>@item.PaymentMethod</td>
              <td class="numeric" style="font-weight:700;color:var(--danger);">@item.Amount.ToString("N2")</td>
              <td><span class="badge badge-success">معتمد</span></td>
              <td class="table-actions"><button class="action-btn edit" onclick="openModal('addExpenseModal')"><i class="fas fa-edit"></i></button><button class="action-btn delete"><i class="fas fa-trash"></i></button></td>
            </tr>"""
    }
]

for p in pages:
    # 1. Generate Interface
    iface_content = f"""using CodeXErpSystem.BLL.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services.Interfaces
{{
    public interface I{p['service']}
    {{
        Task<IEnumerable<{p['model']}ViewModel>> GetAllAsync();
    }}
}}
"""
    with open(os.path.join(iface_dir, f"I{p['service']}.cs"), "w", encoding="utf-8") as f:
        f.write(iface_content)
        
    # 2. Generate Service
    svc_content = f"""using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.Services
{{
    public class {p['service']} : I{p['service']}
    {{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public {p['service']}(IUnitOfWork unitOfWork, IMapper mapper)
        {{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }}

        public async Task<IEnumerable<{p['model']}ViewModel>> GetAllAsync()
        {{
            var entities = await _unitOfWork.GetRepository<{p['model']}>().GetAll(false);
            return _mapper.Map<IEnumerable<{p['model']}ViewModel>>(entities);
        }}
    }}
}}
"""
    with open(os.path.join(svc_dir, f"{p['service']}.cs"), "w", encoding="utf-8") as f:
        f.write(svc_content)

    # 3. Generate ViewModel
    vm_content = f"""using System;

namespace CodeXErpSystem.BLL.ViewModels
{{
    public class {p['model']}ViewModel
    {{
"""
    for prop in p['vm_props']:
        vm_content += f"        {prop}\n"
    vm_content += """    }
}
"""
    with open(os.path.join(vm_dir, f"{p['model']}ViewModel.cs"), "w", encoding="utf-8") as f:
        f.write(vm_content)
        
    # 4. Generate Controller
    ctrl_content = f"""using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{{
    public class {p['controller']} : Controller
    {{
        private readonly I{p['service']} _{p['service'][:1].lower() + p['service'][1:]};

        public {p['controller']}(I{p['service']} {p['service'][:1].lower() + p['service'][1:]})
        {{
            _{p['service'][:1].lower() + p['service'][1:]} = {p['service'][:1].lower() + p['service'][1:]};
        }}

        public async Task<IActionResult> Index()
        {{
            var model = await _{p['service'][:1].lower() + p['service'][1:]}.GetAllAsync();
            return View(model);
        }}
    }}
}}
"""
    with open(os.path.join(controllers_dir, f"{p['controller']}.cs"), "w", encoding="utf-8") as f:
        f.write(ctrl_content)
        
    # 5. Modify View
    html_path = os.path.join(html_dir, p['html_file'])
    if os.path.exists(html_path):
        with open(html_path, "r", encoding="utf-8") as f:
            html = f.read()
            
        # replace <tbody>...</tbody> with the loop
        tbody_match = re.search(r"<tbody>(.*?)</tbody>", html, re.DOTALL)
        if tbody_match:
            new_tbody = "<tbody>\n@foreach(var item in Model) {\n" + p['row_template'] + "\n}\n          </tbody>"
            html = html.replace(tbody_match.group(0), new_tbody)
            
        # prepend model
        html = f"@model IEnumerable<CodeXErpSystem.BLL.ViewModels.{p['model']}ViewModel>\n" + html
        
        # fix hrefs from `../Home/Index.html` to `~/{Controller}/Index`
        html = re.sub(r'href="\.\./\.\./wwwroot/([^"]+)"', r'href="~/\1"', html)
        html = re.sub(r'src="\.\./\.\./wwwroot/([^"]+)"', r'src="~/\1"', html)
        html = re.sub(r'href="\.\./([^/]+)/([^"]+)\.html"', r'href="~/\1/\2"', html)
        
        # specific controller directory
        v_folder = os.path.join(view_dir, p['view_folder'])
        os.makedirs(v_folder, exist_ok=True)
        cshtml_path = os.path.join(v_folder, "Index.cshtml")
        with open(cshtml_path, "w", encoding="utf-8") as f:
            f.write(html)
            
        # 6. Delete original file
        os.remove(html_path)

print("Done")
