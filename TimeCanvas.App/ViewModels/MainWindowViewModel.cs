using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeCanvas.Core.Services;

namespace TimeCanvas.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ITaskService _taskService;
    private readonly StatsService _statsService;
    private readonly SettingsService _settingsService;
    
    public event EventHandler? SettingsRequested;
    public event EventHandler? HistoryRequested;
    public event EventHandler? TemplatesRequested;

    public ObservableCollection<TaskItemViewModel> Tasks { get; } = [];

    public ObservableCollection<DayButtonViewModel> WeekDates { get; } = [];

    [ObservableProperty]
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

    [ObservableProperty]
    private double _completionPercent;
    
    [ObservableProperty]
    private double? _efficiencyPercent;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private int _totalCount;

    public MainWindowViewModel(ITaskService taskService, StatsService statsService, SettingsService settingsService)
    {
        _taskService = taskService;
        _statsService = statsService;
        _settingsService = settingsService;
        SyncWeekDates();
    }

    partial void OnSelectedDateChanged(DateOnly value)
    {
        SyncWeekDates();
        _ = LoadAsync();
    }
    
    private void SyncWeekDates()
    {
        var offsetFromSunday = (int)SelectedDate.DayOfWeek;
        var sunday = SelectedDate.AddDays(-offsetFromSunday);

        // Only rebuild the 7 buttons if we've actually moved to a different week.
        if (WeekDates.Count != 7 || WeekDates[0].Date != sunday)
        {
            WeekDates.Clear();
            for (var i = 0; i < 7; i++) WeekDates.Add(new DayButtonViewModel(sunday.AddDays(i)));
        }
    
        foreach (var day in WeekDates) day.IsSelected = day.Date == SelectedDate;
    }
    
    [RelayCommand]
    private void OpenSettings() => SettingsRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void OpenHistory() => HistoryRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void OpenTemplates() => TemplatesRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            foreach (var existing in Tasks) existing.Dispose();
            Tasks.Clear();

            var items = await _taskService.GetTasksForDateAsync(SelectedDate);
            foreach (var item in items)
            {
                var vm = new TaskItemViewModel(item, _taskService);
                vm.Persisted += async (_, _) => await RefreshStatsAsync();
                Tasks.Add(vm);
            }

            await RefreshStatsAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToToday() => SelectedDate = DateOnly.FromDateTime(DateTime.Today);

    [RelayCommand]
    private void SelectDate(DateOnly date) => SelectedDate = date;

    [RelayCommand]
    private async Task AddTaskAsync()
    {
        var settings = await _settingsService.LoadAsync();

        var defaultStart = Tasks.Count > 0
            ? Tasks[^1].PlannedStart.Add(TimeSpan.FromMinutes(Tasks[^1].PlannedDurationMinutes))
            : settings.DefaultDayStartTime.ToTimeSpan();

        var created = await _taskService.AddTaskAsync(
            SelectedDate, TimeOnly.FromTimeSpan(defaultStart), settings.DefaultTaskDurationMinutes, "");

        var vm = new TaskItemViewModel(created, _taskService);
        vm.Persisted += async (_, _) => await RefreshStatsAsync();
        Tasks.Add(vm);

        await RefreshStatsAsync();
    }

    [RelayCommand]
    private async Task DeleteTaskAsync(TaskItemViewModel task)
    {
        await _taskService.DeleteTaskAsync(task.Id);
        Tasks.Remove(task);
        task.Dispose();
        await RefreshStatsAsync();
    }

    [RelayCommand]
    private async Task NewFromTemplateAsync()
    {
        await _taskService.SeedFromTemplateAsync(SelectedDate, SelectedDate.DayOfWeek);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task CopyFromYesterdayAsync()
    {
        await _taskService.SeedFromDateAsync(SelectedDate, SelectedDate.AddDays(-1));
        await LoadAsync();
    }

    private async Task RefreshStatsAsync()
    {
        var stats = await _statsService.GetStatsForDateAsync(SelectedDate);
        CompletionPercent = stats.CompletionPercent;
        EfficiencyPercent = stats.EfficiencyPercent;
        CompletedCount = stats.CompletedTasks;
        TotalCount = stats.TotalTasks;
    }
}
