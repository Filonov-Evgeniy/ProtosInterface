namespace OZTM.Data.Tables;

public partial class OperationType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();
}
