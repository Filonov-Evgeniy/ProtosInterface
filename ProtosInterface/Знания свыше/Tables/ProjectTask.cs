namespace OZTM.Data.Tables;

public partial class ProjectTask
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int ProjectId { get; set; }
    public int OwnerId { get; set; }
    public int ExecutorId { get; set; }
    public int SolverModeId { get; set; }
    public int WorkShiftId { get; set; }
    public bool Status { get; set; }
    public bool AreaType { get; set; }
    public string? Description { get; set; }

    public virtual User Executor { get; set; } = null!;
    public virtual User Owner { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<ProjectTaskVariant> ProjectTaskVariants { get; set; } = new List<ProjectTaskVariant>();
    public virtual SolverMode SolverMode { get; set; } = null!;
    public virtual WorkShift WorkShift { get; set; } = null!;
}
