namespace OZTM.Data.Tables;

public partial class Equipment
{
    public int Id { get; set; }
    public string InventoryNumber { get; set; } = null!;
    public string? ShortName { get; set; }
    public string Name { get; set; } = null!;
    public int FunctionalAreaId { get; set; }
    public int TerritorialAreaId { get; set; }
    public double LoadFactor { get; set; }
    public int? WorkShiftId { get; set; }
    public double Overtime { get; set; }
    public string? Description { get; set; }

    public virtual TerritorialArea Area { get; set; } = null!;
    public virtual ICollection<OperationVariantComponent> OperationVariantComponents { get; set; } = new List<OperationVariantComponent>();
    public virtual FunctionalArea Type { get; set; } = null!;
}
