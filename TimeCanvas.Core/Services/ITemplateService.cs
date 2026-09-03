using TimeCanvas.Core.Models;

namespace TimeCanvas.Core.Services;

public interface ITemplateService
{
    Task<List<TemplateTaskItem>> GetTemplateTasksAsync(DayOfWeek weekday);
    Task<TemplateTaskItem> AddTemplateTaskAsync(DayOfWeek weekday, TimeOnly start, int durationMinutes, string description);
    Task UpdateTemplateTaskAsync(TemplateTaskItem task);
    Task RemoveTemplateTaskAsync(int templateTaskId);
}
