namespace TimeCanvas.Core.Models;

public class AppSettings
{
    public TimeOnly DefaultDayStartTime { get; set; } = new(9, 0);
    public int DefaultTaskDurationMinutes { get; set; } = 30;
    public bool AutoCarryIncompleteTasks { get; set; } = false;
}
