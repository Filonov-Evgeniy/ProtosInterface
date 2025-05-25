namespace OZTM.Data.Tables;

public partial class DataAccess
{
    public string TableName { get; set; } = null!;
    public int RecordId { get; set; }
    public int UserId { get; set; }
    public DateTime AccessTime { get; set; }
    public string? Description { get; set; }

    public virtual User User { get; set; } = null!;
}
