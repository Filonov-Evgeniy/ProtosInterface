namespace OZTM.Data.Tables;

public partial class Operation
{
    public int Id { get; set; }
    public int Code { get; set; }
    public int TypeId { get; set; }
    public int ProductId { get; set; }
    public int CoopStatusId { get; set; }
    public string? Description { get; set; }

    public virtual CoopStatus CoopStatus { get; set; } = null!;
    public virtual ICollection<OperationVariant> OperationVariants { get; set; } = new List<OperationVariant>();
    public virtual Product Product { get; set; } = null!;
    public virtual OperationType Type { get; set; } = null!;
}
