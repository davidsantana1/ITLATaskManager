using Microsoft.EntityFrameworkCore;

namespace ITLATaskManager.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Models.ToDoTask<string>> ToDoTasks { get; set; }
    }
}
