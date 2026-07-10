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
    public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
    {
        public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
        {
            builder.HasOne(j => j.Account)
                .WithMany(a => a.JournalEntryLines)
                .HasForeignKey(j => j.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(j => j.JournalEntry)
                .WithMany(j => j.JournalEntryLines)
                .HasForeignKey(j => j.JournalEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
