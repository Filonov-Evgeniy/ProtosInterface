namespace OZTM.Data.Tables;

public partial class Profession
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<OperationVariantComponent> OperationVariantComponents { get; set; } = new List<OperationVariantComponent>();
}
