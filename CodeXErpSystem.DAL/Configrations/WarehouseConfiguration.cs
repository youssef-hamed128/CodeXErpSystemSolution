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
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
                    builder.HasOne(w => w.Manager)
            .WithMany(a => a.ManagedWarehouses)
            .HasForeignKey(a => a.MangerId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
