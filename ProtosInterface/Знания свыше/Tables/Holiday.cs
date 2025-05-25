namespace OZTM.Data.Tables;

public partial class Holiday
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int TypeId { get; set; }
    public int ExtraHours { get; set; }
    public string? Description { get; set; }

    public virtual HolidayType Type { get; set; } = null!;
}
