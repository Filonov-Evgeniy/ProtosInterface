namespace OZTM.Data.Tables;

public partial class OperationVariant
{
    public int Id { get; set; }
    public int OperationId { get; set; }
    public double Duration { get; set; }
    public string? Description { get; set; }

    public virtual Operation Operation { get; set; } = null!;
    public virtual ICollection<OperationVariantComponent> OperationVariantComponents { get; set; } = new List<OperationVariantComponent>();
}
