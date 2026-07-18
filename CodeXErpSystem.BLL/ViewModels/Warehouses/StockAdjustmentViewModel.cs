namespace CodeXErpSystem.BLL.ViewModels.Warehouses
{
    public class StockAdjustmentViewModel
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }

        public System.Collections.Generic.IEnumerable<WarehouseViewModel> Warehouses { get; set; } = new System.Collections.Generic.List<WarehouseViewModel>();
        public System.Collections.Generic.IEnumerable<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel> Products { get; set; } = new System.Collections.Generic.List<CodeXErpSystem.BLL.ViewModels.Products.ProductViewModel>();
        public System.Collections.Generic.IEnumerable<StockAdjustmentItemViewModel> AdjustmentItems { get; set; } = new System.Collections.Generic.List<StockAdjustmentItemViewModel>();
    }
}