using System;
using System.ComponentModel.DataAnnotations;

namespace CodeXErpSystem.BLL.ViewModels.Products
{
    public class ProductCreateViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        
        [Required(ErrorMessage = "اسم الصنف مطلوب")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "الفئة مطلوبة")]
        public int CategoryId { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون الرقم موجباً")]
        public decimal PurchasePrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون الرقم موجباً")]
        public decimal SalePrice { get; set; }
        
        public decimal TaxRate { get; set; }
        public bool HasStockTracking { get; set; } = true;
        public string? UnitOfMeasure { get; set; }
        public decimal MinStockLevel { get; set; } = 1;
    }
}
