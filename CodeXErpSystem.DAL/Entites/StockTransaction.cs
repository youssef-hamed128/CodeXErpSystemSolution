using CodeXErpSystem.DAL.Entites.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class StockTransaction : BaseEntity
    {
        public decimal Quantity { get; set; }
        public StockTransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string? ReferenceId { get; set; }
        public string? Note { get; set; }
        public Product Product { get; set; } = null!;
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Warehouse? SourceWarehouse { get; set; } = null!;
        [ForeignKey("SourceWarehouseId")]
        public int? SourceWarehouseId { get; set; }
        public Warehouse? DestWarehouse { get; set; } = null!;
        [ForeignKey("DestWarehouseId")]
        public int? DestWarehouseId { get; set; }


    }
}
