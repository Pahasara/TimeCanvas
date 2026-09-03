using TimeCanvas.Core.Data;
using TimeCanvas.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace TimeCanvas.Core.Services;

public class TemplateService(IDbContextFactory<TimeCanvasDbContext> contextFactory) : ITemplateService
{
    public async Task<List<TemplateTaskItem>> GetTemplateTasksAsync(DayOfWeek weekday)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var template = await GetOrCreateTemplateAsync(db, weekday);

        return await db.TemplateTasks
            .Where(t => t.DayTemplateId == template.Id)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }

    public async Task<TemplateTaskItem> AddTemplateTaskAsync(
        DayOfWeek weekday, TimeOnly start, int durationMinutes, string description)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var template = await GetOrCreateTemplateAsync(db, weekday);

        var nextOrder = await db.TemplateTasks
            .Where(t => t.DayTemplateId == template.Id)
            .Select(t => (int?)t.SortOrder)
            .MaxAsync() ?? 0;

        var task = new TemplateTaskItem
        {
            DayTemplateId = template.Id,
            PlannedStart = start,
            PlannedDurationMinutes = durationMinutes,
            Description = description,
            SortOrder = nextOrder + 1,
        };

        db.TemplateTasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }
    
    public async Task UpdateTemplateTaskAsync(TemplateTaskItem task)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var existing = await db.TemplateTasks.FindAsync(task.Id)
            ?? throw new InvalidOperationException($"Template task {task.Id} not found.");

        existing.PlannedStart = task.PlannedStart;
        existing.PlannedDurationMinutes = task.PlannedDurationMinutes;
        existing.Description = task.Description;
        await db.SaveChangesAsync();
    }

    public async Task RemoveTemplateTaskAsync(int templateTaskId)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        await db.TemplateTasks.Where(t => t.Id == templateTaskId).ExecuteDeleteAsync();
    }

    private static async Task<DayTemplate> GetOrCreateTemplateAsync(TimeCanvasDbContext db, DayOfWeek weekday)
    {
        var template = await db.DayTemplates.FirstOrDefaultAsync(t => t.Weekday == weekday);
        if (template is not null) return template;

        template = new DayTemplate { Weekday = weekday };
        db.DayTemplates.Add(template);
        await db.SaveChangesAsync();
        return template;
    }
}
