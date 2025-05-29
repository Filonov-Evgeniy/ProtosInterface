using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGraph.models
{
    public partial class Operation
    {
        public int Id { get; set; }
        public int Code { get; set; }

        [Column("Type_id")]
        public int TypeId { get; set; }

        [Column("Product_Id")]
        public int ProductId { get; set; }

        [Column("Coop_Status_Id")]
        public int CoopStatusId { get; set; }
        public string? Description { get; set; }

        //public virtual CoopStatus CoopStatus { get; set; } = null!;
        public virtual ICollection<OperationVariant> OperationVariants { get; set; } = new List<OperationVariant>();
        public virtual Product Product { get; set; } = null!;
        public virtual OperationType Type { get; set; } = null!;

    }
}
