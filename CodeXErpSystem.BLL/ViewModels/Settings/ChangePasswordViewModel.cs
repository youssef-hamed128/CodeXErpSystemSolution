using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Settings
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        public string OldPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "6 أحرف على الأقل")]
        public string NewPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
