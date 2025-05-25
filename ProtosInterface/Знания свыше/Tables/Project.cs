namespace OZTM.Data.Tables;

public partial class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int OwnerId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public bool Status { get; set; }
    public string? Description { get; set; }

    public virtual User Owner { get; set; } = null!;
    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
