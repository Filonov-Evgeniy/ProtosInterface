namespace OZTM.Data.Tables;

public partial class TerritorialArea
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public double Overtime { get; set; }
    public int WorkShiftId { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    public virtual WorkShift WorkShift { get; set; } = null!;
    public virtual ICollection<ProjectTaskVariant> TaskVariants { get; set; } = new List<ProjectTaskVariant>();
    //public virtual ICollection<ProjectTaskVariantTerritorialArea> ProjectTaskVariantTerritorialAreas { get; set; } = new List<ProjectTaskVariantTerritorialArea>();
}
