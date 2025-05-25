namespace OZTM.Data.Tables;

public partial class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
