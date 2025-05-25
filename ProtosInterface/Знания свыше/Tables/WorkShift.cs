namespace OZTM.Data.Tables;

public partial class WorkShift
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public double Duration { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<FunctionalArea> FunctionalAreas { get; set; } = new List<FunctionalArea>();
    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    public virtual ICollection<TerritorialArea> TerritorialAreas { get; set; } = new List<TerritorialArea>();
}
