using Microsoft.EntityFrameworkCore;
using TimeCanvas.Core.Data;
using TimeCanvas.Core.Models;

namespace TimeCanvas.Core.Services;

public class StatsService(IDbContextFactory<TimeCanvasDbContext> contextFactory)
{
    public async Task<DayStats> GetStatsForDateAsync(DateOnly date)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var tasks = await db.Tasks.Where(t => t.Date == date).ToListAsync();
        return BuildStats(date, tasks);
    }

    public async Task<List<DayStats>> GetTrendAsync(DateOnly from, DateOnly to)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var tasks = await db.Tasks
            .Where(t => t.Date >= from && t.Date <= to)
            .ToListAsync();

        return tasks
            .GroupBy(t => t.Date)
            .Select(g => BuildStats(g.Key, [.. g]))
            .OrderBy(s => s.Date)
            .ToList();
    }

    private static DayStats BuildStats(DateOnly date, List<TaskItem> tasks)
    {
        var completed = tasks.Where(t => t.IsCompleted).ToList();
        var withLoggedTime = completed.Where(t => t.ActualMinutesUsed is not null).ToList();

        return new DayStats(
            Date: date,
            TotalTasks: tasks.Count,
            CompletedTasks: completed.Count,
            PlannedMinutes: tasks.Sum(t => t.PlannedDurationMinutes),
            LoggedActualMinutes: withLoggedTime.Sum(t => t.ActualMinutesUsed!.Value))
        {
            PlannedMinutesForCompleted = withLoggedTime.Sum(t => t.PlannedDurationMinutes),
        };
    }
}
