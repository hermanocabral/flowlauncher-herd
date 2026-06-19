using System.IO;
using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>Resolves the effective target and working directory for an <see cref="AppEntry"/>.</summary>
public static class PathResolver
{
    /// <summary>Expands <c>%ENV%</c> references; returns an empty string for null input.</summary>
    public static string Expand(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : Environment.ExpandEnvironmentVariables(value);

    /// <summary>True when the target carries a URI scheme (e.g. <c>https://</c>, <c>obsidian://</c>).</summary>
    public static bool IsUrl(string target) =>
        !string.IsNullOrEmpty(target) && target.Contains("://", StringComparison.Ordinal);

    /// <summary>
    /// Returns the working directory to launch the entry from: the explicit value when set,
    /// otherwise the target file's own folder. Null when there is no sensible directory
    /// (URLs, or a bare command resolved via PATH).
    /// </summary>
    public static string? ResolveWorkingDirectory(AppEntry entry)
    {
        var custom = Expand(entry.WorkingDirectory);
        if (!string.IsNullOrWhiteSpace(custom))
        {
            return custom;
        }

        var target = Expand(entry.Target);
        if (string.IsNullOrWhiteSpace(target) || IsUrl(target))
        {
            return null;
        }

        // A folder target starts in the folder itself, not its parent.
        if (Directory.Exists(target))
        {
            return target;
        }

        var directory = Path.GetDirectoryName(target);
        return string.IsNullOrEmpty(directory) ? null : directory;
    }
}
