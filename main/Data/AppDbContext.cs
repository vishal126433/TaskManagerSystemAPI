using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define your DbSets here
        public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TaskType> Types { get; set; }
        public DbSet<TaskPriority> Priority { get; set; }

    }
}
