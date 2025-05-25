namespace OZTM.Data.Tables;

public partial class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int DepartmentId { get; set; }
    public int PostId { get; set; }

    public virtual Department Department { get; set; } = null!;
    public virtual Post Post { get; set; } = null!;
    public virtual ICollection<ProjectTask> ProjectTaskExecutors { get; set; } = new List<ProjectTask>();
    public virtual ICollection<ProjectTask> ProjectTaskOwners { get; set; } = new List<ProjectTask>();
    public virtual ICollection<ProjectTaskVariant> ProjectTaskVariantExecutors { get; set; } = new List<ProjectTaskVariant>();
    public virtual ICollection<ProjectTaskVariant> ProjectTaskVariantOwners { get; set; } = new List<ProjectTaskVariant>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
