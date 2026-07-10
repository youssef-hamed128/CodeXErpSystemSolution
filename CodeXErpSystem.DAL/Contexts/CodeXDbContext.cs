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



    }
}
