using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Flow.Launcher.Plugin.Herd.Views;

/// <summary>Maps an integer count to Visibility: visible when zero (e.g. empty-list hints).</summary>
public sealed class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isZero = value is int count && count == 0;
        return isZero ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
