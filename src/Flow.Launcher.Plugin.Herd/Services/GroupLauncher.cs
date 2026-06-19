using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>
/// Launches every enabled app in a group. In <see cref="LaunchMode.Sequential"/> mode it
/// pauses <see cref="AppGroup.DelayMs"/> between launches; a single failure never aborts
/// the rest. The delay is injectable so timing is deterministic in tests.
/// </summary>
public sealed class GroupLauncher
{
    private readonly IProcessLauncher _launcher;
    private readonly Func<int, Task> _delay;

    public GroupLauncher(IProcessLauncher launcher, Func<int, Task>? delay = null)
    {
        _launcher = launcher;
        _delay = delay ?? (ms => Task.Delay(ms));
    }

    /// <summary>Builds a launch-ready request from an entry, resolving paths and working directory.</summary>
    public static LaunchRequest BuildRequest(AppEntry entry) => new(
        FileName: PathResolver.Expand(entry.Target),
        Arguments: entry.Arguments,
        WorkingDirectory: PathResolver.ResolveWorkingDirectory(entry),
        RunAsAdmin: entry.RunAsAdmin);

    /// <summary>Launches the group's enabled apps and reports how many started and which failed.</summary>
    public async Task<LaunchResult> LaunchAsync(AppGroup group)
    {
        var apps = group.EnabledApps.ToList();
        var failures = new List<LaunchFailure>();
        var launched = 0;

        for (var i = 0; i < apps.Count; i++)
        {
            try
            {
                _launcher.Launch(BuildRequest(apps[i]));
                launched++;
            }
            catch (OperationCanceledException)
            {
                // User declined elevation — not launched, but not an error to report.
            }
            catch (Exception ex)
            {
                failures.Add(new LaunchFailure(apps[i], ex));
            }

            var hasNext = i < apps.Count - 1;
            if (group.LaunchMode == LaunchMode.Sequential && hasNext)
            {
                await _delay(Math.Max(0, group.DelayMs));
            }
        }

        return new LaunchResult(launched, failures);
    }
}
