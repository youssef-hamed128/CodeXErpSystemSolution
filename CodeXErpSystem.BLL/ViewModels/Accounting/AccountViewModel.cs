using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Accounting
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "كود الحساب مطلوب")]
        public string Code { get; set; } = string.Empty;
        [Required(ErrorMessage = "اسم الحساب مطلوب")]
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
