using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeCanvas.Core.Services;

namespace TimeCanvas.ViewModels;

public partial class SettingsViewModel(SettingsService settingsService) : ObservableObject
{
    [ObservableProperty] private TimeSpan _defaultDayStartTime;
    [ObservableProperty] private int _defaultTaskDurationMinutes;
    [ObservableProperty] private bool _autoCarryIncompleteTasks;

    public event EventHandler? Saved;

    [RelayCommand]
    private async Task LoadAsync()
    {
        var settings = await settingsService.LoadAsync();
        DefaultDayStartTime = settings.DefaultDayStartTime.ToTimeSpan();
        DefaultTaskDurationMinutes = settings.DefaultTaskDurationMinutes;
        AutoCarryIncompleteTasks = settings.AutoCarryIncompleteTasks;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await settingsService.SaveAsync(new()
        {
            DefaultDayStartTime = TimeOnly.FromTimeSpan(DefaultDayStartTime),
            DefaultTaskDurationMinutes = DefaultTaskDurationMinutes,
            AutoCarryIncompleteTasks = AutoCarryIncompleteTasks,
        });
        Saved?.Invoke(this, EventArgs.Empty);
    }
}
