namespace TimeCanvas.Core.Models;

public class TemplateTaskItem
{
    public int Id { get; set; }

    public int DayTemplateId { get; set; }
    public DayTemplate? DayTemplate { get; set; }

    public required TimeOnly PlannedStart { get; set; }
    public required int PlannedDurationMinutes { get; set; }
    public required string Description { get; set; }
    public int SortOrder { get; set; }
}
