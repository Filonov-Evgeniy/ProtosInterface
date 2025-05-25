namespace OZTM.Data.Tables;

public partial class CoopMode
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<ProjectTaskVariant> ProjectTaskVariants { get; set; } = new List<ProjectTaskVariant>();
}
