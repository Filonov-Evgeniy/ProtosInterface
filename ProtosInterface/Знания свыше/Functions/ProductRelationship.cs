using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OZTM.Data.Views;

[Keyless]
public class ProductRelationship
{
    [Column("final_product_id")]
    public int FinalProductId { get; set; }
    [Column("parent_product_id")]
    public int ParentProductId { get; set; }
    [Column("included_product_id")]
    public int IncludedProductId { get; set; }
    [Column("amount")]
    public double Amount { get; set; }
    [Column("total_amount")]
    public double TotalAmount { get; set; }
    [Column("technology_exists")]
    public bool TechnologyExists { get; set; }
}