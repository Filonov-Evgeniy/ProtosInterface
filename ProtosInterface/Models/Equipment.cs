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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("inventory_number")]
        [MaxLength(10)]
        public string InventoryNumber { get; set; }

        [Column("short_name")]
        [MaxLength(100)]
        public string? ShortName { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [Column("functional_area_id")]
        public int FunctionalAreaId { get; set; }

        [Required]
        [Column("territorial_area_id")]
        public int TerritorialAreaId { get; set; }

        [Required]
        [Column("load_factor")]
        public double LoadFactor { get; set; } = 1.0;

        [Column("work_shift_id")]
        public int? WorkShiftId { get; set; }

        [Column("overtime")]
        public double? Overtime { get; set; } = 0.0;

        [Column("description")]
        [MaxLength(4000)]
        public string? Description { get; set; }

        //[ForeignKey("TypeId")]
        //public Equipment_Type EquipmentType { get; set; }
    }
}
