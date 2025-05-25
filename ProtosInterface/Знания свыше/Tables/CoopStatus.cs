namespace OZTM.Data.Tables;

public partial class CoopStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
