using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGraph.models
{
    public partial class Equipment
    {
        public int Id { get; set; }

        [Column("inventory_number")]
        public string InventoryNumber { get; set; } = null!;

        [Column("short_name")]
        public string? ShortName { get; set; }
        public string Name { get; set; } = null!;

        [Column("functional_area_id")]
        public int FunctionalAreaId { get; set; }

        [Column("territorial_area_id")]
        public int TerritorialAreaId { get; set; }

        [Column("load_factor")]
        public double LoadFactor { get; set; }

        [Column("work_shift_id")]
        public int? WorkShiftId { get; set; }
        public double Overtime { get; set; }
        public string? Description { get; set; }

        //public virtual TerritorialArea Area { get; set; } = null!;
        public virtual ICollection<OperationVariantComponent> OperationVariantComponents { get; set; } = new List<OperationVariantComponent>();
        //public virtual FunctionalArea Type { get; set; } = null!;
    }
}
