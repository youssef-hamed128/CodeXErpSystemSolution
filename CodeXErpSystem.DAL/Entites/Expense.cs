using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class Expense : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string Category { get; set; } = null!;
        public string? Description { get; set; }
        public string PaymentMethod { get; set; } = null!;
    }
}
