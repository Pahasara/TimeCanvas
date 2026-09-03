using TimeCanvas.Core.Services;

namespace TimeCanvas.Core.Tests;

public class TaskServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();
    private readonly TaskService _sut;

    public TaskServiceTests() => _sut = new TaskService(_factory);

    [Fact]
    public async Task AddTaskAsync_PersistsAndReturnsTask()
    {
        var date = new DateOnly(2026, 8, 28);

        var task = await _sut.AddTaskAsync(date, new TimeOnly(9, 0), 60, "Write tests");
        var tasksForDate = await _sut.GetTasksForDateAsync(date);

        Assert.Single(tasksForDate);
        Assert.Equal("Write tests", tasksForDate[0].Description);
        Assert.Equal(task.Id, tasksForDate[0].Id);
    }

    [Fact]
    public async Task CompleteTaskAsync_SetsCompletionAndActualMinutes()
    {
        var date = new DateOnly(2026, 8, 28);
        var task = await _sut.AddTaskAsync(date, new TimeOnly(9, 0), 60, "Deep work");

        var completed = await _sut.CompleteTaskAsync(task.Id, 45);

        Assert.True(completed.IsCompleted);
        Assert.Equal(45, completed.ActualMinutesUsed);
        Assert.Equal(0.75, completed.EfficiencyRatio);
    }

    public void Dispose() => _factory.Dispose();
}
