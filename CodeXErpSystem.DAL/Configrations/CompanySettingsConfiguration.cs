using CodeXErpSystem.DAL.Entites; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeXErpSystem.DAL.Configrations
{
    public class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
    {
        public void Configure(EntityTypeBuilder<CompanySettings> builder)
        {
            builder.ToTable("CompanySettings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TaxNumber).HasMaxLength(50);
            builder.Property(x => x.CRNumber).HasMaxLength(50);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.Email).HasMaxLength(150);
            builder.Property(x => x.Address).HasMaxLength(500);


            builder.Property(x => x.BaseCurrency).HasMaxLength(10).HasDefaultValue("SAR");


            builder.Property(x => x.DefaultTaxRate).HasColumnType("decimal(18,2)");
        }
    }
}