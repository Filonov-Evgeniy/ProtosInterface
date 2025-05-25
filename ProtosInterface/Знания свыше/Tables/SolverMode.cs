namespace OZTM.Data.Tables;

public partial class SolverMode
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
}
