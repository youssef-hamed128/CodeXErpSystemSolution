using CodeXErpSystem.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeXErpSystem.DAL.Configrations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Category).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PaymentMethod).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(1000);

            // تحديد دقة الأرقام العشرية للمبلغ
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        }
    }
}