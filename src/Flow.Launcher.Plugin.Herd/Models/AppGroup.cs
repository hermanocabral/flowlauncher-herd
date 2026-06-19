using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Herd.Models;

/// <summary>A named set of applications launched together.</summary>
public class AppGroup : ObservableObject
{
    private string _name = string.Empty;
    private string? _description;
    private string? _iconPath;
    private LaunchMode _launchMode = LaunchMode.Parallel;
    private int _delayMs = 200;

    /// <summary>Stable identifier, also used as the result key.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Group name shown in the launcher and matched against the query.</summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>Free-text description shown in the result subtitle and preview panel.</summary>
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>Optional custom icon path; falls back to the plugin icon when empty.</summary>
    public string? IconPath
    {
        get => _iconPath;
        set => SetProperty(ref _iconPath, value);
    }

    /// <summary>Whether apps launch all at once or one after another.</summary>
    public LaunchMode LaunchMode
    {
        get => _launchMode;
        set => SetProperty(ref _launchMode, value);
    }

    /// <summary>Delay between launches (milliseconds) when <see cref="LaunchMode"/> is Sequential.</summary>
    public int DelayMs
    {
        get => _delayMs;
        set => SetProperty(ref _delayMs, value);
    }

    /// <summary>The apps in this group, in launch order.</summary>
    public ObservableCollection<AppEntry> Apps { get; set; } = new();

    /// <summary>Apps that are enabled, in order.</summary>
    [JsonIgnore]
    public IEnumerable<AppEntry> EnabledApps => Apps.Where(a => a.Enabled);

    /// <summary>Returns a deep copy with a fresh <see cref="Id"/> and cloned apps.</summary>
    public AppGroup Clone()
    {
        var clone = new AppGroup
        {
            Name = Name,
            Description = Description,
            IconPath = IconPath,
            LaunchMode = LaunchMode,
            DelayMs = DelayMs,
        };

        foreach (var app in Apps)
        {
            clone.Apps.Add(app.Clone());
        }

        return clone;
    }
}
