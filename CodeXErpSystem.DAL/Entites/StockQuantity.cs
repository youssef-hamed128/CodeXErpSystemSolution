using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class StockQuantity : BaseEntity
    {
        public decimal Quantity { get; set; }
        public Product Product { get; set; } = null!;
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        [ForeignKey("WarehouseId")]
        public int WarehouseId { get; set; }


    }
}
