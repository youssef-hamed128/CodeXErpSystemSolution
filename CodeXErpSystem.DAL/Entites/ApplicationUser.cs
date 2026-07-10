using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites
{
    public class ApplicationUser : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Role? Role { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public ICollection<Warehouse> ManagedWarehouses { get; set; } = new List<Warehouse>();
    }
}
