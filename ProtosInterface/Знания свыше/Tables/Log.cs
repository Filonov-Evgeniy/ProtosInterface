//namespace OZTM.Data.Tables;

//public partial class Log
//{
//    public int Id { get; set; }
//    public string Type { get; set; } = null!;
//    public string? Object { get; set; }
//    public int? ObjectId { get; set; }
//    public string Action { get; set; } = null!;
//    public int ExecutorId { get; set; }
//    public DateTime Time { get; set; }
//    public string Status { get; set; } = null!;
//    public string? Description { get; set; }
//    public string? Data { get; set; }

//    public virtual User Executor { get; set; } = null!;

//    public Log(string Type, string? Object, int? ObjectId, string Action, int ExecutorId, string Status, string? Description, DBContext context)
//    {
//        this.Id = 0;
//        this.Type = Type;
//        this.Object = Object;
//        this.ObjectId = ObjectId;
//        this.Action = Action;
//        this.ExecutorId = ExecutorId;
//        this.Time = DateTime.Now;
//        this.Status = Status;
//        this.Description = Description;

//        this.AddRecord(context);
//    }

//    private void AddRecord(DBContext context)
//    {
//        context.Logs.Add(this);
//        context.SaveChanges();
//    }
//}
