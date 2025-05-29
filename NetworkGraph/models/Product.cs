using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGraph.models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Column("Type_id")]
        public int TypeId { get; set; }

        [Column("Coop_Status_Id")]
        public int CoopStatusId { get; set; }
        public string? Description { get; set; }

        //public virtual CoopStatus CoopStatus { get; set; } = null!;
        public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();
        public virtual ICollection<ProductLink> ProductLinkIncludedProducts { get; set; } = new List<ProductLink>();
        public virtual ICollection<ProductLink> ProductLinkParentProducts { get; set; } = new List<ProductLink>();
        //public virtual ProductType Type { get; set; } = null!;
        //public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
