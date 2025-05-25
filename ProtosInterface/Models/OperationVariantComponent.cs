using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Models
{
    internal class OperationVariantComponent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OperationVariantId { get; set; }

        public int EquipmentId { get; set; }

        public int ProfessionId { get; set; }

        public int WorkersAmount { get; set; }
 
        public string? Description { get; set; }

        [ForeignKey("OperationVariantId")]
        public OperationVariant OperationVariant { get; set; }

        [ForeignKey("EquipmentId")]
        public Equipment Equipment { get; set; }
    }
}
