namespace OZTM.Data.Tables;

public partial class Limitation
{
    public int Id { get; set; }
    public int TaskVariantId { get; set; }
    public int LeftBoundTypeId { get; set; }
    public int? LeftElementId { get; set; }
    public double LeftValue { get; set; }
    public int TypeId { get; set; }
    public double RightValue { get; set; }
    public int? RightElementId { get; set; }
    public int RightBoundTypeId { get; set; }

    public virtual LimitationBoundType LeftBoundType { get; set; } = null!;
    public virtual LimitationBoundType RightBoundType { get; set; } = null!;
    public virtual ProjectTaskVariant TaskVariant { get; set; } = null!;
    public virtual LimitationType Type { get; set; } = null!;
}
