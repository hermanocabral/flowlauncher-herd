using System.ComponentModel;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>
/// Real launcher. Uses shell execution so executables, documents, folders and URLs
/// all work, and so elevation via the <c>runas</c> verb is available.
/// </summary>
public sealed class ProcessLauncher : IProcessLauncher
{
    // Windows ERROR_CANCELLED — the user dismissed the UAC elevation prompt.
    private const int ElevationCancelled = 1223;

    public void Launch(LaunchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new InvalidOperationException("No target specified for this app.");
        }

        var info = new ProcessStartInfo
        {
            FileName = request.FileName,
            UseShellExecute = true,
        };

        if (!string.IsNullOrEmpty(request.Arguments))
        {
            info.Arguments = request.Arguments;
        }

        if (!string.IsNullOrEmpty(request.WorkingDirectory))
        {
            info.WorkingDirectory = request.WorkingDirectory;
        }

        if (request.RunAsAdmin)
        {
            info.Verb = "runas";
        }

        try
        {
            Process.Start(info);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == ElevationCancelled)
        {
            // The user declined the UAC prompt — a deliberate choice, not a launch failure.
            throw new OperationCanceledException($"Elevation was cancelled for {request.FileName}.", ex);
        }
    }
}
