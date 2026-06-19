using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>A resolved, launch-ready description of a single app start.</summary>
public sealed record LaunchRequest(
    string FileName,
    string? Arguments,
    string? WorkingDirectory,
    bool RunAsAdmin);

/// <summary>A single app that failed to launch, with the underlying error.</summary>
public sealed record LaunchFailure(AppEntry Entry, Exception Error);

/// <summary>Outcome of launching a group: how many started and which ones failed.</summary>
public sealed record LaunchResult(int Launched, IReadOnlyList<LaunchFailure> Failures);
