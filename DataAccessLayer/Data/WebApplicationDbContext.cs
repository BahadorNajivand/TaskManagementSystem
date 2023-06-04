using Microsoft.EntityFrameworkCore;
using Task = ModelsAndEnums.Models.Task;
using ModelsAndEnums.Models;

public class WebApplicationDbContext : DbContext
{
    public WebApplicationDbContext(DbContextOptions<WebApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
