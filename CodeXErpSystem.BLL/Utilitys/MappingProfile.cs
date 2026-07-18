using AutoMapper;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Accounting;
using CodeXErpSystem.BLL.ViewModels.Customers;
using CodeXErpSystem.BLL.ViewModels.Expenses;
using CodeXErpSystem.BLL.ViewModels.Invoice;
using CodeXErpSystem.BLL.ViewModels.Payments;
using CodeXErpSystem.BLL.ViewModels.Products;
using CodeXErpSystem.BLL.ViewModels.Settings;
using CodeXErpSystem.BLL.ViewModels.Suppliers;
using CodeXErpSystem.BLL.ViewModels.Warehouses;
using CodeXErpSystem.DAL.Entites; // في حال كانت الجداول هنا
// تأكد من الـ namespace الخاص بجداول قاعدة البيانات

namespace CodeXErpSystem.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. Users & Settings
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserCreateViewModel>().ReverseMap();
            CreateMap<CompanySettings, CompanySettingsViewModel>().ReverseMap();

            // 2. Inventory & Products
            CreateMap<Product, ProductViewModel>().ReverseMap();
            CreateMap<Product, ProductCreateViewModel>().ReverseMap();

            CreateMap<ProductCategory, ProductCategoryViewModel>().ReverseMap();

            CreateMap<Warehouse, WarehouseViewModel>().ReverseMap();

            // 3. Parties
            CreateMap<Customer, CustomerViewModel>().ReverseMap();
            CreateMap<Supplier, SupplierViewModel>().ReverseMap();

            // 4. Finance & Accounting
            CreateMap<Expense, ExpenseViewModel>().ReverseMap();
            CreateMap<Expense, ExpenseCreateViewModel>().ReverseMap();
            CreateMap<Account, AccountViewModel>().ReverseMap();

            // 5. Invoices
            CreateMap<Invoice, InvoiceViewModel>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
                .ReverseMap();

            CreateMap<Invoice, InvoiceCreateViewModel>().ReverseMap();

            CreateMap<InvoiceItem, InvoiceItemViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Code : null))
                .ReverseMap();

            CreateMap<InvoiceItem, InvoiceItemCreateViewModel>().ReverseMap();

            // 6. Payments
            CreateMap<Payment, PaymentViewModel>().ReverseMap();

            CreateMap<Payment, PaymentCreateViewModel>().ReverseMap();

            // 7. Journal Entries
            CreateMap<JournalEntry, JournalEntryViewModel>().ReverseMap();
            CreateMap<JournalEntryLine, JournalEntryLineViewModel>()
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : null))
                .ReverseMap();
        }
    }
}
