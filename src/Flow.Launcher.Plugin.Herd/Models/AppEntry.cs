using System.IO;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Herd.Models;

/// <summary>A single application (or file/folder/URL) launched as part of a group.</summary>
public class AppEntry : ObservableObject
{
    private string _target = string.Empty;
    private string? _displayName;
    private string? _arguments;
    private string? _workingDirectory;
    private bool _runAsAdmin;
    private bool _enabled = true;

    /// <summary>Path to an executable/file/folder, or a URL. Required.</summary>
    public string Target
    {
        get => _target;
        set
        {
            if (SetProperty(ref _target, value))
            {
                OnPropertyChanged(nameof(DisplayLabel));
            }
        }
    }

    /// <summary>Optional friendly name; falls back to the target's file name (see <see cref="DisplayLabel"/>).</summary>
    public string? DisplayName
    {
        get => _displayName;
        set
        {
            if (SetProperty(ref _displayName, value))
            {
                OnPropertyChanged(nameof(DisplayLabel));
            }
        }
    }

    /// <summary>Command-line arguments passed to the target.</summary>
    public string? Arguments
    {
        get => _arguments;
        set => SetProperty(ref _arguments, value);
    }

    /// <summary>Working directory; when empty the target's own folder is used.</summary>
    public string? WorkingDirectory
    {
        get => _workingDirectory;
        set => SetProperty(ref _workingDirectory, value);
    }

    /// <summary>Launch the target elevated (triggers a UAC prompt).</summary>
    public bool RunAsAdmin
    {
        get => _runAsAdmin;
        set => SetProperty(ref _runAsAdmin, value);
    }

    /// <summary>When false the entry is kept in the group but skipped at launch.</summary>
    public bool Enabled
    {
        get => _enabled;
        set => SetProperty(ref _enabled, value);
    }

    /// <summary>Returns an independent copy of this entry.</summary>
    public AppEntry Clone() => new()
    {
        Target = Target,
        DisplayName = DisplayName,
        Arguments = Arguments,
        WorkingDirectory = WorkingDirectory,
        RunAsAdmin = RunAsAdmin,
        Enabled = Enabled,
    };

    /// <summary>Human-readable label: the display name when set, otherwise derived from the target.</summary>
    [JsonIgnore]
    public string DisplayLabel
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(DisplayName))
            {
                return DisplayName!;
            }

            if (string.IsNullOrWhiteSpace(Target))
            {
                return "(empty)";
            }

            if (Target.Contains("://", StringComparison.Ordinal))
            {
                return Target;
            }

            var trimmed = Target.TrimEnd('\\', '/');
            var name = Path.GetFileNameWithoutExtension(trimmed);
            return string.IsNullOrEmpty(name) ? trimmed : name;
        }
    }
}
