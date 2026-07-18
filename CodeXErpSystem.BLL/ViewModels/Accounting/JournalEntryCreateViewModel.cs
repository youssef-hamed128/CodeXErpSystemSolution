using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Accounting
{
    public class JournalEntryCreateViewModel
    {
        [Required(ErrorMessage = "تاريخ القيد مطلوب")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public List<JournalEntryLineCreateViewModel> Lines { get; set; } = new();
    }
}
