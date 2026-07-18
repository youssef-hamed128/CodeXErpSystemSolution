using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Accounting
{
    public class JournalEntryLineCreateViewModel
    {
        [Required(ErrorMessage = "الحساب مطلوب")]
        public int AccountId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Debit { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Credit { get; set; }
        public string? Description { get; set; }

    }
}
