using TimeCanvas.Core.Services;

namespace TimeCanvas.Core.Tests;

public class StatsServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();
    private readonly TaskService _taskService;
    private readonly StatsService _sut;

    public StatsServiceTests()
    {
        _taskService = new TaskService(_factory);
        _sut = new StatsService(_factory);
    }

    [Fact]
    public async Task GetStatsForDateAsync_ComputesCompletionAndEfficiency()
    {
        var date = new DateOnly(2026, 8, 28);
        var t1 = await _taskService.AddTaskAsync(date, new TimeOnly(9, 0), 60, "Task 1");
        await _taskService.AddTaskAsync(date, new TimeOnly(10, 0), 30, "Task 2 — left incomplete");
        await _taskService.CompleteTaskAsync(t1.Id, 30); // used half the planned time

        var stats = await _sut.GetStatsForDateAsync(date);

        Assert.Equal(2, stats.TotalTasks);
        Assert.Equal(1, stats.CompletedTasks);
        Assert.Equal(50, stats.CompletionPercent);
        Assert.Equal(50, stats.EfficiencyPercent); // 30 actual / 60 planned, over completed tasks only
    }

    public void Dispose() => _factory.Dispose();
}
