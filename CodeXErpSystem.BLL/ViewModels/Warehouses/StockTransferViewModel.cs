using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class StockTransferViewModel
    {
        [Required(ErrorMessage = "الصنف مطلوب")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "مخزن المصدر مطلوب")]
        public int SourceWarehouseId { get; set; }
        [Required(ErrorMessage = "مخزن الوجهة مطلوب")]
        public int DestWarehouseId { get; set; }
        [Range(0.0001, double.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }

        public IEnumerable<WarehouseViewModel> Warehouses { get; set; } = new List<WarehouseViewModel>();
        public IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel> Products { get; set; } = new List<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel>();
        public IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductCategoryViewModel> Categories { get; set; } = new List<CodeXErpSystem.BLL.ViewModels.Products.ProductCategoryViewModel>();
        public IEnumerable<StockTransferItemViewModel> TransferItems { get; set; } = new List<StockTransferItemViewModel>();
    }
}
