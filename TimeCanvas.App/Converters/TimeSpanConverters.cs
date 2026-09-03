using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TimeCanvas.Converters;

public class NullableTimeSpanConverter : IValueConverter
{
    public static readonly NullableTimeSpanConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value as TimeSpan?;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value as TimeSpan? ?? TimeSpan.Zero;
}

public class MinutesToDecimalConverter : IValueConverter
{
    public static readonly MinutesToDecimalConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is int minutes ? (decimal)minutes : 0m;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is decimal d ? (int)d : 0;
}
