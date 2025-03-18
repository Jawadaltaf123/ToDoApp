using Microsoft.EntityFrameworkCore;
using WebAPI.Models.ModelClasses;


namespace WebAPI.Models.Data
{
    public class EmployeeDbContext: DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskDto> Tasks { get; set; }

        
    }
}
