using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Models
{
    internal class OperationVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OperationId { get; set; }

        public int Duration { get; set; }

        public string? Description { get; set; }

        [ForeignKey("OperationId")]
        public Operation Operation { get; set; }
    }
}
