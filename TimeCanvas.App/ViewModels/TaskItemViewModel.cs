using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeCanvas.Core.Models;
using TimeCanvas.Core.Services;

namespace TimeCanvas.ViewModels;

public partial class TaskItemViewModel : ObservableObject, IDisposable
{
    private readonly ITaskService _taskService;
    private readonly int _id;
    private readonly DateOnly _date;
    private readonly System.Timers.Timer _saveDebounce;

    public static IReadOnlyList<string> EfficiencyLabels { get; } = ["25%", "50%", "75%", "100%", "125%"];

    public event EventHandler? Persisted;

    [ObservableProperty] private TimeSpan _plannedStart;
    [ObservableProperty] private int _plannedDurationMinutes;
    [ObservableProperty] private string _description;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private int? _actualMinutesUsed;
    [ObservableProperty] private string _selectedEfficiencyLabel = "100%";

    public int SortOrder { get; }
    public int Id => _id;

    public TaskItemViewModel(TaskItem model, ITaskService taskService)
    {
        _taskService = taskService;
        _id = model.Id;
        _date = model.Date;
        SortOrder = model.SortOrder;

        _plannedStart = model.PlannedStart.ToTimeSpan();
        _plannedDurationMinutes = model.PlannedDurationMinutes;
        _description = model.Description;
        _isCompleted = model.IsCompleted;
        _actualMinutesUsed = model.ActualMinutesUsed;

        _selectedEfficiencyLabel = model.ActualMinutesUsed is { } actual && model.PlannedDurationMinutes > 0
            ? NearestLabel(actual / (double)model.PlannedDurationMinutes * 100)
            : "100%";

        _saveDebounce = new System.Timers.Timer(600) { AutoReset = false };
        _saveDebounce.Elapsed += async (_, _) => await SaveAsync();
    }

    public double? EfficiencyPercent =>
        IsCompleted && ActualMinutesUsed is { } actual && PlannedDurationMinutes > 0
            ? actual / (double)PlannedDurationMinutes * 100
            : null;

    partial void OnPlannedStartChanged(TimeSpan value) => ScheduleSave();
    partial void OnPlannedDurationMinutesChanged(int value) => ScheduleSave();
    partial void OnDescriptionChanged(string value) => ScheduleSave();

    // Discrete actions, not typing — save immediately, no debounce.
    partial void OnIsCompletedChanged(bool value) => _ = PersistCompletionAsync(value);

    partial void OnSelectedEfficiencyLabelChanged(string value)
    {
        // Only meaningful once completed — picking a value before checking
        // the box just pre-sets what will be used when you do.
        if (IsCompleted) _ = PersistCompletionAsync(true);
    }

    private void ScheduleSave()
    {
        _saveDebounce.Stop();
        _saveDebounce.Start();
    }

    private async Task SaveAsync()
    {
        await _taskService.UpdateTaskAsync(new TaskItem
        {
            Id = _id,
            Date = _date,
            PlannedStart = TimeOnly.FromTimeSpan(PlannedStart),
            PlannedDurationMinutes = PlannedDurationMinutes,
            Description = Description,
            IsCompleted = IsCompleted,
            ActualMinutesUsed = ActualMinutesUsed,
            SortOrder = SortOrder,
        });

        Persisted?.Invoke(this, EventArgs.Empty);
    }

    private async Task PersistCompletionAsync(bool completed)
    {
        TaskItem updated;
        if (completed)
        {
            var minutes = (int)Math.Round(PlannedDurationMinutes * ParsePercent(SelectedEfficiencyLabel) / 100.0);
            updated = await _taskService.CompleteTaskAsync(_id, minutes);
        }
        else
        {
            updated = await _taskService.SetCompletedAsync(_id, false);
        }

        ActualMinutesUsed = updated.ActualMinutesUsed;
        OnPropertyChanged(nameof(EfficiencyPercent));
        Persisted?.Invoke(this, EventArgs.Empty);
    }

    private static int ParsePercent(string label) => int.Parse(label.TrimEnd('%'));

    private static string NearestLabel(double percent) =>
        EfficiencyLabels.OrderBy(l => Math.Abs(ParsePercent(l) - percent)).First();

    public void Dispose() => _saveDebounce.Dispose();
}
