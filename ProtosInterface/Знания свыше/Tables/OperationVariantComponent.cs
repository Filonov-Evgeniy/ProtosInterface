namespace OZTM.Data.Tables;

public partial class OperationVariantComponent
{
    public int Id { get; set; }
    public int OperationVariantId { get; set; }
    public int? EquipmentId { get; set; }
    public int? ProfessionId { get; set; }
    public int? WorkersAmount { get; set; }

    public virtual Equipment? Equipment { get; set; }
    public virtual OperationVariant OperationVariant { get; set; } = null!;
    public virtual Profession? Profession { get; set; }
}
