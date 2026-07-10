using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public ApplicationUser? Manager { get; set; }
        [ForeignKey("MangerId")]
        public int? MangerId { get; set; }
        public ICollection<StockQuantity> StockQuantities { get; set; } = new List<StockQuantity>();
        public ICollection<StockTransaction> SourceTransactions { get; set; } = new List<StockTransaction>();
        public ICollection<StockTransaction> DestTransactions { get; set; } = new List<StockTransaction>();


    }
}
