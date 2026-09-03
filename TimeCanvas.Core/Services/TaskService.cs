using Microsoft.EntityFrameworkCore;
using TimeCanvas.Core.Data;
using TimeCanvas.Core.Models;

namespace TimeCanvas.Core.Services;

public class TaskService(IDbContextFactory<TimeCanvasDbContext> contextFactory) : ITaskService
{
    public async Task<List<TaskItem>> GetTasksForDateAsync(DateOnly date)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        return await db.Tasks
            .Where(t => t.Date == date)
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.PlannedStart)
            .ToListAsync();
    }

    public async Task<TaskItem> AddTaskAsync(
        DateOnly date, TimeOnly plannedStart, int plannedDurationMinutes, string description)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var nextSortOrder = await db.Tasks
            .Where(t => t.Date == date)
            .Select(t => (int?)t.SortOrder)
            .MaxAsync() ?? 0;

        var task = new TaskItem
        {
            Date = date,
            PlannedStart = plannedStart,
            PlannedDurationMinutes = plannedDurationMinutes,
            Description = description,
            SortOrder = nextSortOrder + 1,
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        db.Tasks.Update(task);
        await db.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        await db.Tasks.Where(t => t.Id == taskId).ExecuteDeleteAsync();
    }

    public async Task<TaskItem> CompleteTaskAsync(int taskId, int actualMinutesUsed)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var task = await db.Tasks.FindAsync(taskId)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.IsCompleted = true;
        task.ActualMinutesUsed = actualMinutesUsed;
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UncompleteTaskAsync(int taskId)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var task = await db.Tasks.FindAsync(taskId)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.IsCompleted = false;
        task.ActualMinutesUsed = null;
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<int> SeedFromTemplateAsync(DateOnly date, DayOfWeek weekday)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var template = await db.DayTemplates
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Weekday == weekday);

        if (template is null || template.Tasks.Count == 0) return 0;

        var newTasks = template.Tasks
            .OrderBy(t => t.SortOrder)
            .Select(t => new TaskItem
            {
                Date = date,
                PlannedStart = t.PlannedStart,
                PlannedDurationMinutes = t.PlannedDurationMinutes,
                Description = t.Description,
                SortOrder = t.SortOrder,
            });

        db.Tasks.AddRange(newTasks);
        return await db.SaveChangesAsync();
    }

    public async Task<int> SeedFromDateAsync(DateOnly targetDate, DateOnly sourceDate, bool resetCompletion = true)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var sourceTasks = await db.Tasks
            .Where(t => t.Date == sourceDate)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        if (sourceTasks.Count == 0) return 0;

        var copies = sourceTasks.Select(t => new TaskItem
        {
            Date = targetDate,
            PlannedStart = t.PlannedStart,
            PlannedDurationMinutes = t.PlannedDurationMinutes,
            Description = t.Description,
            SortOrder = t.SortOrder,
            IsCompleted = !resetCompletion && t.IsCompleted,
            ActualMinutesUsed = resetCompletion ? null : t.ActualMinutesUsed,
        });

        db.Tasks.AddRange(copies);
        return await db.SaveChangesAsync();
    }
    
    public async Task<TaskItem> SetCompletedAsync(int taskId, bool isCompleted)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var task = await db.Tasks.FindAsync(taskId)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.IsCompleted = isCompleted;
        task.ActualMinutesUsed = null; // no longer tracked
        await db.SaveChangesAsync();
        return task;
    }
}
