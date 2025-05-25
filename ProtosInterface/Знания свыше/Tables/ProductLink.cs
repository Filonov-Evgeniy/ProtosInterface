namespace OZTM.Data.Tables;

public partial class ProductLink
{
    public int ParentProductId { get; set; }
    public int IncludedProductId { get; set; }
    public double Amount { get; set; }

    public virtual Product IncludedProduct { get; set; } = null!;
    public virtual Product ParentProduct { get; set; } = null!;
}
