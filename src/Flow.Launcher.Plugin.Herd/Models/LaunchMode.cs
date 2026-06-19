using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Herd.Models;

/// <summary>How the apps in a group are started when the group is launched.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LaunchMode
{
    /// <summary>Start every app at once.</summary>
    Parallel,

    /// <summary>Start apps one after another, pausing <see cref="AppGroup.DelayMs"/> between each.</summary>
    Sequential,
}
