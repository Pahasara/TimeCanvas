using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TimeCanvas.Converters;

/// <summary>Maps 0–100% to 0–80 pixels for the trend bar chart.</summary>
public class PercentToHeightConverter : IValueConverter
{
    public static readonly PercentToHeightConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            double d => Math.Clamp(d, 0, 100) / 100.0 * 80.0,
            _ => 2.0, // thin sliver for a day with no data
        };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
