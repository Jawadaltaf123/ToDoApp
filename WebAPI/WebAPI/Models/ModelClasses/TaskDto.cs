namespace WebAPI.Models.ModelClasses
{
    public class TaskDto: BaseClass
    {
        public long Id { get; set; }
        public string TaskTitle { get; set; }
        public bool IsImportant { get; set; }
        public bool IsCompleted { get; set; }
        public long UserId { get; set; }


        //public virtual User? User { get; set; }

    }
}
