using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeCanvas.Core.Models;
using TimeCanvas.Core.Services;

namespace TimeCanvas.ViewModels;

public partial class TemplateTaskRowViewModel : ObservableObject, IDisposable
{
    private readonly ITemplateService _templateService;
    private readonly System.Timers.Timer _saveDebounce;

    public int Id { get; }

    [ObservableProperty] private TimeSpan _plannedStart;
    [ObservableProperty] private int _plannedDurationMinutes;
    [ObservableProperty] private string _description;

    public TemplateTaskRowViewModel(TemplateTaskItem model, ITemplateService templateService)
    {
        _templateService = templateService;
        Id = model.Id;
        _plannedStart = model.PlannedStart.ToTimeSpan();
        _plannedDurationMinutes = model.PlannedDurationMinutes;
        _description = model.Description;

        _saveDebounce = new System.Timers.Timer(600) { AutoReset = false };
        _saveDebounce.Elapsed += async (_, _) => await SaveAsync();
    }

    partial void OnPlannedStartChanged(TimeSpan value) => ScheduleSave();
    partial void OnPlannedDurationMinutesChanged(int value) => ScheduleSave();
    partial void OnDescriptionChanged(string value) => ScheduleSave();

    private void ScheduleSave()
    {
        _saveDebounce.Stop();
        _saveDebounce.Start();
    }

    private async Task SaveAsync() => await _templateService.UpdateTemplateTaskAsync(new TemplateTaskItem
    {
        Id = Id,
        PlannedStart = TimeOnly.FromTimeSpan(PlannedStart),
        PlannedDurationMinutes = PlannedDurationMinutes,
        Description = Description,
    });

    public void Dispose() => _saveDebounce.Dispose();
}
