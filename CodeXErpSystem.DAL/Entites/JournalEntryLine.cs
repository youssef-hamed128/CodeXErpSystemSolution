using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class JournalEntryLine : BaseEntity
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public JournalEntry? JournalEntry { get; set; } = null!;
        [ForeignKey("JournalEntryId")]
        public int JournalEntryId { get; set; }
        public Account? Account { get; set; } = null!;
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }

    }
}
