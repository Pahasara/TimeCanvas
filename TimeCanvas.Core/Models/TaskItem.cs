namespace TimeCanvas.Core.Models;

public class TaskItem
{
    public int Id { get; set; }
    public required DateOnly Date { get; set; }
    public required TimeOnly PlannedStart { get; set; }
    public required int PlannedDurationMinutes { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public int? ActualMinutesUsed { get; set; }
    public int SortOrder { get; set; }
    
    public TimeOnly PlannedEnd => PlannedStart.AddMinutes(PlannedDurationMinutes);

    public double? EfficiencyRatio =>
        IsCompleted && ActualMinutesUsed is { } actual && PlannedDurationMinutes > 0
            ? actual / (double)PlannedDurationMinutes
            : null;
}
