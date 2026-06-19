using System.Diagnostics;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>
/// Real launcher. Uses shell execution so executables, documents, folders and URLs
/// all work, and so elevation via the <c>runas</c> verb is available.
/// </summary>
public sealed class ProcessLauncher : IProcessLauncher
{
    public void Launch(LaunchRequest request)
    {
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

        Process.Start(info);
    }
}
