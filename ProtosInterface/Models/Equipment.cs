using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Models
{
    internal class Equipment
    {
        [Key]
        public int Id { get; set; }

        public int InventoryNumber { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(30)]
        public string AreaId { get; set; }

        [Required]
        public int TypeId { get; set; }

        public int LoadFactor { get; set; }

        public string? Description { get; set; }

        [MaxLength(10)]
        public string ShortName { get; set; }

        //[ForeignKey("TypeId")]
        //public Equipment_Type EquipmentType { get; set; }
    }
}
