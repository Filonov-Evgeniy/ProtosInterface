using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGraph.models
{
    public partial class OperationVariantComponent
    {
        public int Id { get; set; }

        [Column("operation_variant_id")]
        public int OperationVariantId { get; set; }

        [Column("equipment_id")]
        public int? EquipmentId { get; set; }

        [Column("Profession_Id")]
        public int? ProfessionId { get; set; }

        [Column("Workers_Amount")]
        public int? WorkersAmount { get; set; }

        [ForeignKey("Equipment_Id")]
        public virtual Equipment? Equipment { get; set; }

        [ForeignKey("Operation_Variant_Id")]
        public virtual OperationVariant OperationVariant { get; set; } = null!;
        //public virtual Profession? Profession { get; set; }
    }
}
