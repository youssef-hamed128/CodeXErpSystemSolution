using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Settings
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "6 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "الصلاحية مطلوبة")]
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
