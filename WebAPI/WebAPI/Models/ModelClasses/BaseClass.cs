namespace WebAPI.Models.ModelClasses
{
    public class BaseClass
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public string CreatedBy { get; set; } 
        //public DateTime? UpdatedAt { get; set; } 
        //public string? UpdatedBy { get; set; } 
     

    }
}
