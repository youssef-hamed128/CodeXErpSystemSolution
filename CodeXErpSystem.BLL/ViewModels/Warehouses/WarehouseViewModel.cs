using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم المخزن مطلوب")]
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        
        // UI specific properties
        public string? Phone { get; set; }
        public string? Code { get; set; }
        public string? Address { get => Location; set => Location = value; }
        public bool IsActive { get; set; } = true;
        public decimal TotalInventoryValue { get; set; }
        public string BgColor { get; set; } = "var(--bg-surface)";
        public string TextColor { get; set; } = "var(--text-main)";
        public string IconClass { get; set; } = "fa-warehouse";
        public string StatusClass { get; set; } = "badge-success";
        public string Status { get; set; } = "نشط";
    }
}
