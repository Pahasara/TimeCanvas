using Microsoft.EntityFrameworkCore;
using TimeCanvas.Core.Models;

namespace TimeCanvas.Core.Data;

public class TimeCanvasDbContext(DbContextOptions<TimeCanvasDbContext> options)
    : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<DayTemplate> DayTemplates => Set<DayTemplate>();
    public DbSet<TemplateTaskItem> TemplateTasks => Set<TemplateTaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasIndex(t => t.Date);
            entity.Property(t => t.Description).HasMaxLength(256);
        });

        modelBuilder.Entity<DayTemplate>(entity =>
        {
            entity.HasIndex(t => t.Weekday).IsUnique();
            entity.HasMany(t => t.Tasks)
                  .WithOne(t => t.DayTemplate)
                  .HasForeignKey(t => t.DayTemplateId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TemplateTaskItem>(entity =>
        {
            entity.Property(t => t.Description).HasMaxLength(256);
        });
    }
}
