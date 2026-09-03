using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeCanvas.ViewModels;

public partial class DayButtonViewModel(DateOnly date) : ObservableObject
{
    public DateOnly Date { get; } = date;
    public string DayLabel { get; } = date.ToString("ddd").ToUpperInvariant(); // "SUN"
    public string DayNumber { get; } = date.Day.ToString();

    [ObservableProperty]
    private bool _isSelected;
}
