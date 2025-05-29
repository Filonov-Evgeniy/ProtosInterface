using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGraph.models
{
    public partial class ProductLink
    {
        [Column("Parent_Product_Id")]
        public int ParentProductId { get; set; }

        [Column("Included_Product_Id")]
        public int IncludedProductId { get; set; }
        public /*decimal*/ double Amount { get; set; }

        public virtual Product IncludedProduct { get; set; } = null!;
        public virtual Product ParentProduct { get; set; } = null!;
    }
}
