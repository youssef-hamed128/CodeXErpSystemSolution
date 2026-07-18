using CodeXErpSystem.BLL.Mapping;
using CodeXErpSystem.BLL.Services.Classes;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.DAL.Contexts;
using CodeXErpSystem.DAL.Repository.Classes;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeXErpSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<CodeXDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            });
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IInvoiceService, InvoiceServices>();
            
            // New Modules Injections
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IJournalEntryService, JournalEntryService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();
            
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IStockTransferService, StockTransferService>();
            builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
