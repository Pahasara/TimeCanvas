namespace TimeCanvas.Core.Services;

public record DayStats(
    DateOnly Date,
    int TotalTasks,
    int CompletedTasks,
    int PlannedMinutes,
    int LoggedActualMinutes)
{
    public double CompletionPercent =>
        TotalTasks == 0 ? 0 : CompletedTasks / (double)TotalTasks * 100;

    public double? EfficiencyPercent =>
        PlannedMinutesForCompleted == 0
            ? null
            : LoggedActualMinutes / (double)PlannedMinutesForCompleted * 100;

    public int PlannedMinutesForCompleted { get; init; }
}
