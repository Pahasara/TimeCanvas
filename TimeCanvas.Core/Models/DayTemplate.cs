namespace TimeCanvas.Core.Models;

public class DayTemplate
{
    public int Id { get; set; }

    public required DayOfWeek Weekday { get; set; }

    public List<TemplateTaskItem> Tasks { get; set; } = [];
}
