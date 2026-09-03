using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeCanvas.Core.Services;

namespace TimeCanvas.ViewModels;

public partial class TemplateEditorViewModel : ObservableObject
{
    private readonly ITemplateService _templateService;

    public ObservableCollection<DayButtonViewModel> Weekdays { get; } = [];
    public ObservableCollection<TemplateTaskRowViewModel> Rows { get; } = [];

    [ObservableProperty]
    private DayOfWeek _selectedWeekday = DateTime.Today.DayOfWeek;

    public TemplateEditorViewModel(ITemplateService templateService)
    {
        _templateService = templateService;

        // Reuses DayButtonViewModel purely for its DayLabel/IsSelected shape —
        // the Date here is just "some day with this weekday," never persisted.
        var today = DateOnly.FromDateTime(DateTime.Today);
        var sunday = today.AddDays(-(int)today.DayOfWeek);
        for (var i = 0; i < 7; i++)
        {
            var date = sunday.AddDays(i);
            var day = new DayButtonViewModel(date) { IsSelected = date.DayOfWeek == SelectedWeekday };
            Weekdays.Add(day);
        }
    }

    partial void OnSelectedWeekdayChanged(DayOfWeek value)
    {
        foreach (var day in Weekdays) day.IsSelected = day.Date.DayOfWeek == value;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void SelectWeekday(DateOnly date) => SelectedWeekday = date.DayOfWeek;

    [RelayCommand]
    private async Task LoadAsync()
    {
        foreach (var row in Rows) row.Dispose();
        Rows.Clear();

        var tasks = await _templateService.GetTemplateTasksAsync(SelectedWeekday);
        foreach (var task in tasks)
            Rows.Add(new TemplateTaskRowViewModel(task, _templateService));
    }

    [RelayCommand]
    private async Task AddRowAsync()
    {
        var defaultStart = Rows.Count > 0
            ? Rows[^1].PlannedStart.Add(TimeSpan.FromMinutes(Rows[^1].PlannedDurationMinutes))
            : TimeSpan.FromHours(9);

        var created = await _templateService.AddTemplateTaskAsync(
            SelectedWeekday, TimeOnly.FromTimeSpan(defaultStart), 30, "");

        Rows.Add(new TemplateTaskRowViewModel(created, _templateService));
    }

    [RelayCommand]
    private async Task RemoveRowAsync(TemplateTaskRowViewModel row)
    {
        await _templateService.RemoveTemplateTaskAsync(row.Id);
        Rows.Remove(row);
        row.Dispose();
    }
}
