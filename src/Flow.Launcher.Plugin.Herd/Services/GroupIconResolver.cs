using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>Which source a group's icon should come from.</summary>
public enum GroupIconKind
{
    /// <summary>The group's explicit custom icon.</summary>
    Custom,

    /// <summary>The icon of the group's first app target.</summary>
    Target,

    /// <summary>The plugin's default (sheep) icon.</summary>
    Default,
}

/// <summary>A resolved icon choice: where to get it and (for Custom/Target) the path.</summary>
public sealed record GroupIconSource(GroupIconKind Kind, string? Path);

/// <summary>
/// Decides which icon represents a group: its custom icon if set, otherwise the first
/// app target's icon, otherwise the default. Pure (filesystem checks are injected) so the
/// decision is unit-testable; turning the choice into an actual image is the converter's job.
/// </summary>
public static class GroupIconResolver
{
    public static GroupIconSource Resolve(AppGroup group, Func<string, bool> fileExists)
    {
        var custom = PathResolver.Expand(group.IconPath);
        if (!string.IsNullOrWhiteSpace(custom) && fileExists(custom))
        {
            return new GroupIconSource(GroupIconKind.Custom, custom);
        }

        var first = group.Apps.FirstOrDefault();
        if (first is not null)
        {
            var target = PathResolver.Expand(first.Target);
            if (!string.IsNullOrWhiteSpace(target) && !PathResolver.IsUrl(target) && fileExists(target))
            {
                return new GroupIconSource(GroupIconKind.Target, target);
            }
        }

        return new GroupIconSource(GroupIconKind.Default, null);
    }
}
