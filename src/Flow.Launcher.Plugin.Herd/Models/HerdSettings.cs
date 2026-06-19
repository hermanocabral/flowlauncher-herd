using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.Herd.Models;

/// <summary>Root object persisted via Flow's settings storage.</summary>
public class HerdSettings
{
    /// <summary>All configured application groups.</summary>
    public ObservableCollection<AppGroup> Groups { get; set; } = new();
}
