using CodeXErpSystem.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Configrations
{
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.HasOne(t=>t.SourceWarehouse)
                .WithMany(w=>w.SourceTransactions)
                .HasForeignKey(t=>t.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.DestWarehouse)
                .WithMany(w => w.DestTransactions)
                .HasForeignKey(t => t.DestWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Product)
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
