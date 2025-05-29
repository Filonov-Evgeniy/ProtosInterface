using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NetworkGraph.models
{
    public partial class OperationVariant
    {
        public int Id { get; set; }

        [Column("Operation_Id")]
        public int OperationId { get; set; }
        public double Duration { get; set; }
        public string? Description { get; set; }

        public virtual Operation Operation { get; set; } = null!;
        public virtual ICollection<OperationVariantComponent> OperationVariantComponents { get; set; } = new List<OperationVariantComponent>();
    }
}
