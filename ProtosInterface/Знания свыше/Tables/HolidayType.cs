namespace OZTM.Data.Tables;

public partial class HolidayType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
}
