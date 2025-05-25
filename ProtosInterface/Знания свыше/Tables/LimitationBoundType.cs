namespace OZTM.Data.Tables;

public partial class LimitationBoundType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<Limitation> LimitationLeftBoundTypes { get; set; } = new List<Limitation>();
    public virtual ICollection<Limitation> LimitationRightBoundTypes { get; set; } = new List<Limitation>();
}
