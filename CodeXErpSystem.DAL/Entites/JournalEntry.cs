using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class JournalEntry  : BaseEntity
    {
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public JournalEntryStatus Status { get; set; }
        public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();

    }
}
