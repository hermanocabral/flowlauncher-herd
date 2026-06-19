namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>
/// Starts a process for a <see cref="LaunchRequest"/>. Abstracted so launch
/// orchestration can be tested without spawning real processes.
/// </summary>
public interface IProcessLauncher
{
    /// <summary>Starts the process described by <paramref name="request"/>. Throws on failure.</summary>
    void Launch(LaunchRequest request);
}
