using TimeCanvas.Core.Models;

namespace TimeCanvas.Core.Services;

public interface ITaskService
{
    Task<List<TaskItem>> GetTasksForDateAsync(DateOnly date);
    Task<TaskItem> AddTaskAsync(DateOnly date, TimeOnly plannedStart, int plannedDurationMinutes, string description);
    Task UpdateTaskAsync(TaskItem task);
    Task DeleteTaskAsync(int taskId);
    Task<TaskItem> CompleteTaskAsync(int taskId, int actualMinutesUsed);
    Task<TaskItem> UncompleteTaskAsync(int taskId);
    Task<int> SeedFromTemplateAsync(DateOnly date, DayOfWeek weekday);
    Task<int> SeedFromDateAsync(DateOnly targetDate, DateOnly sourceDate, bool resetCompletion = true);
    Task<TaskItem> SetCompletedAsync(int taskId, bool isCompleted);
}
