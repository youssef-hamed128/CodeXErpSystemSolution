using CodeXErpSystem.DAL.Configrations;
using CodeXErpSystem.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Contexts
{
    public class CodeXDbContext : DbContext
    {
        public CodeXDbContext(DbContextOptions<CodeXDbContext>options) : base (options)
        {
 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Invoice>(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration<Payment>(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration<Account>(new AccountConfiguration());
            modelBuilder.ApplyConfiguration<ApplicationUser>(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration<JournalEntryLine>(new JournalEntryLineConfiguration());
            modelBuilder.ApplyConfiguration<Warehouse>(new WarehouseConfiguration());
            modelBuilder.ApplyConfiguration<StockTransaction>(new StockTransactionConfiguration());
            modelBuilder.ApplyConfiguration<CompanySettings>(new CompanySettingsConfiguration());
            modelBuilder.ApplyConfiguration<Expense>(new ExpenseConfiguration());

            // Seed Roles
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "مدير النظام", CreatedAt = seedDate },
                new Role { Id = 2, Name = "محاسب", CreatedAt = seedDate },
                new Role { Id = 3, Name = "مبيعات", CreatedAt = seedDate }
            );
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<JournalEntry> JournalEntrys { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<StockQuantity> StockQuantities { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<Expense> Expenses { get; set; }




    }
}
