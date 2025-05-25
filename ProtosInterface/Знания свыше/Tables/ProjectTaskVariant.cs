namespace OZTM.Data.Tables;

public partial class ProjectTaskVariant
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int TaskId { get; set; }
    public int OwnerId { get; set; }
    public int ExecutorId { get; set; }
    public int TimeModeId { get; set; }
    public int CoopModeId { get; set; }
    public bool Status { get; set; }
    public string? Description { get; set; }

    public virtual CoopMode CoopMode { get; set; } = null!;
    public virtual User Executor { get; set; } = null!;
    public virtual ICollection<Limitation> Limitations { get; set; } = new List<Limitation>();
    public virtual User Owner { get; set; } = null!;
    public virtual ProjectTask Task { get; set; } = null!;
    public virtual TimeMode TimeMode { get; set; } = null!;
    public virtual ICollection<TerritorialArea> TerritorialAreas { get; set; } = new List<TerritorialArea>();
    public virtual ICollection<FunctionalArea> FunctionalAreas { get; set; } = new List<FunctionalArea>();
    //public virtual ICollection<ProjectTaskVariantFunctionalArea> ProjectTaskVariantFunctionalAreas { get; set; } = new List<ProjectTaskVariantFunctionalArea>();
}
