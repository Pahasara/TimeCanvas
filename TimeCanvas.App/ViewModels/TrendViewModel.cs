using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeCanvas.Core.Services;

namespace TimeCanvas.ViewModels;

public partial class TrendViewModel(StatsService statsService) : ObservableObject
{
    public ObservableCollection<DayStats> Days { get; } = [];

    [ObservableProperty] private double _averageCompletion;
    [ObservableProperty] private double _averageEfficiency;

    [RelayCommand]
    private async Task LoadAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var trend = await statsService.GetTrendAsync(today.AddDays(-13), today);

        Days.Clear();
        for (var d = today.AddDays(-13); d <= today; d = d.AddDays(1))
        {
            // Fill in gaps for days with zero tasks so the bar chart has a
            // consistent 14-day width instead of skipping empty days.
            Days.Add(trend.FirstOrDefault(s => s.Date == d) ?? new DayStats(d, 0, 0, 0, 0));
        }

        var daysWithTasks = Days.Where(d => d.TotalTasks > 0).ToList();
        AverageCompletion = daysWithTasks.Count == 0 ? 0 : daysWithTasks.Average(d => d.CompletionPercent);
        AverageEfficiency = daysWithTasks
            .Where(d => d.EfficiencyPercent is not null)
            .Select(d => d.EfficiencyPercent!.Value)
            .DefaultIfEmpty(0)
            .Average();
    }
}
