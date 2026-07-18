using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string? Barcode { get; set; }
        [Required(ErrorMessage = "اسم الصنف مطلوب")]
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون الرقم موجباً")]
        public decimal PurchasePrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون الرقم موجباً")]
        public decimal SalePrice { get; set; }
        public decimal TaxRate { get; set; }
        public bool HasStockTracking { get; set; } = true;
        public string? UnitOfMeasure { get; set; }
        public decimal MinStockLevel { get; set; } = 5;

        // UI specific properties mapped from Entity
        public string? SKU { get => Barcode; set => Barcode = value; }
        public string ProductName { get => Name; set => Name = value; }
        public string? Category { get => CategoryName; set => CategoryName = value; }
        public string? Unit { get => UnitOfMeasure; set => UnitOfMeasure = value; }
        public decimal UnitCost { get => PurchasePrice; set => PurchasePrice = value; }
        public decimal MinQuantity { get => MinStockLevel; set => MinStockLevel = value; }
        
        // Calculated properties
        public decimal AvailableQuantity { get; set; }
        public string Status { get; set; } = "متاح";
    }
}
