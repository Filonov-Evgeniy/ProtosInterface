using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Models
{
    class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public int TypeId { get; set; }

        public int CoopStatusId { get; set; }
        [MaxLength(4000)]
        public string? Description { get; set; }

        // Навигационные свойства для связей
        public ICollection<ProductLink> ParentLinks { get; set; } // Где этот продукт является родителем
        public ICollection<ProductLink> ChildLinks { get; set; }
    }
}
