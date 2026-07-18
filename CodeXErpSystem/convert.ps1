$files = @(
    "Views\Reports\Index.html",
    "Views\Reports\BalanceSheet.html",
    "Views\Reports\InventoryReport.html",
    "Views\Reports\View.html",
    "Views\Settings\Index.html",
    "Views\Users\Index.html",
    "Views\Auth\Login.html"
)

foreach ($f in $files) {
    if (Test-Path $f) {
        $content = Get-Content $f -Raw
        $content = "@{`r`n    Layout = null;`r`n}`r`n" + $content
        $content = $content -replace "\.\./\.\./wwwroot/", "~/"
        $content = $content -replace 'href="\.\./([A-Za-z0-9_]+)/([A-Za-z0-9_]+)\.html"', 'href="~/$1/$2"'
        
        # Specific replacements
        $content = $content -replace "window\.location\.href='BalanceSheet\.html'", "window.location.href='@Url.Action(""BalanceSheet"", ""Reports"")'"
        $content = $content -replace "window\.location\.href='InventoryReport\.html'", "window.location.href='@Url.Action(""InventoryReport"", ""Reports"")'"
        $content = $content -replace "window\.location\.href='View\.html\?id=([A-Za-z0-9_]+)'", "window.location.href='@Url.Action(""View"", ""Reports"", new { id = ""`$1"" })'"
        $content = $content -replace 'href="Index\.html"', 'href="~/Reports/Index"'
        $content = $content -replace 'action="\.\./Home/Index\.html"', 'action="~/Home/Index"'
        
        $newFile = $f -replace "\.html$", ".cshtml"
        Set-Content -Path $newFile -Value $content -Encoding UTF8
        Remove-Item -Path $f
        Write-Host "Converted $f to $newFile"
    } else {
        Write-Host "File not found: $f"
    }
}

$reportsController = @"
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index() { return View(); }
        public IActionResult BalanceSheet() { return View(); }
        public IActionResult InventoryReport() { return View(); }
        
        [ActionName("View")]
        public IActionResult ReportView(string id) { return View("View"); }
    }
}
"@
Set-Content "Controllers\ReportsController.cs" $reportsController -Encoding UTF8

$settingsController = @"
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index() { return View(); }
    }
}
"@
Set-Content "Controllers\SettingsController.cs" $settingsController -Encoding UTF8

$usersController = @"
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index() { return View(); }
    }
}
"@
Set-Content "Controllers\UsersController.cs" $usersController -Encoding UTF8

$authController = @"
using Microsoft.AspNetCore.Mvc;

namespace CodeXErpSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login() { return View(); }
    }
}
"@
Set-Content "Controllers\AuthController.cs" $authController -Encoding UTF8
Write-Host "Controllers created successfully."
