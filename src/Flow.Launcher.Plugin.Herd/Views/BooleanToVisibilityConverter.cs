using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Flow.Launcher.Plugin.Herd.Views;

/// <summary>Maps a bool to Visibility; set <see cref="Invert"/> to flip the mapping.</summary>
public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var flag = value is true;
        if (Invert)
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
