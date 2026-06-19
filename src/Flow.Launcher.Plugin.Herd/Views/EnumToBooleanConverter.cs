using System.Globalization;
using System.Windows.Data;

namespace Flow.Launcher.Plugin.Herd.Views;

/// <summary>
/// Binds a radio button's IsChecked to an enum value. ConverterParameter is the enum
/// member name; the radio is checked when the bound value equals it, and checking the
/// radio writes that value back.
/// </summary>
public sealed class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.ToString() == parameter as string;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true && parameter is string name
            ? Enum.Parse(targetType, name)
            : Binding.DoNothing;
}
