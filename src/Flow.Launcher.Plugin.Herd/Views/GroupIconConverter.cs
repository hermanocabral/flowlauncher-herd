using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd.Views;

/// <summary>
/// Produces a group's list icon: its custom icon, else the first app target's extracted icon,
/// else the default sheep. Any failure falls back to the default. Bind via a MultiBinding that
/// includes the group plus its IconPath/Apps.Count so the image refreshes when those change.
/// </summary>
public sealed class GroupIconConverter : IMultiValueConverter
{
    private static ImageSource? _default;

    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        var group = values.OfType<AppGroup>().FirstOrDefault();
        if (group is null)
        {
            return DefaultIcon();
        }

        try
        {
            var source = GroupIconResolver.Resolve(group, File.Exists);
            return source.Kind switch
            {
                GroupIconKind.Custom => LoadImage(source.Path!) ?? DefaultIcon(),
                GroupIconKind.Target => ExtractIcon(source.Path!) ?? DefaultIcon(),
                _ => DefaultIcon(),
            };
        }
        catch
        {
            return DefaultIcon();
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    private static ImageSource? LoadImage(string path)
    {
        try
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // don't keep the file locked
            image.UriSource = new Uri(path, UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch
        {
            return null;
        }
    }

    private static ImageSource? ExtractIcon(string path)
    {
        try
        {
            using var icon = Icon.ExtractAssociatedIcon(path);
            if (icon is null)
            {
                return null;
            }

            var source = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            source.Freeze();
            return source;
        }
        catch
        {
            return null;
        }
    }

    private static ImageSource? DefaultIcon()
    {
        if (_default is not null)
        {
            return _default;
        }

        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (dir is null)
        {
            return null;
        }

        _default = LoadImage(Path.Combine(dir, "Images", "icon.png"));
        return _default;
    }
}
