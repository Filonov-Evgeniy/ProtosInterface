namespace OZTM.Data.Tables;

public partial class LimitationType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<Limitation> Limitations { get; set; } = new List<Limitation>();
}
