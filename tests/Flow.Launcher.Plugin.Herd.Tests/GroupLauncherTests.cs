using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd.Tests;

/// <summary>Records launch requests instead of starting processes; can be told to fail for a target.</summary>
internal sealed class FakeProcessLauncher : IProcessLauncher
{
    public List<LaunchRequest> Launched { get; } = new();
    public HashSet<string> FailFor { get; } = new();

    public void Launch(LaunchRequest request)
    {
        if (FailFor.Contains(request.FileName))
        {
            throw new InvalidOperationException($"cannot launch {request.FileName}");
        }

        Launched.Add(request);
    }
}

public class GroupLauncherTests
{
    private static AppGroup GroupWith(LaunchMode mode, int delayMs, params AppEntry[] apps)
    {
        var group = new AppGroup { LaunchMode = mode, DelayMs = delayMs };
        foreach (var app in apps)
        {
            group.Apps.Add(app);
        }

        return group;
    }

    [Test]
    public async Task Parallel_launches_enabled_and_skips_disabled()
    {
        var fake = new FakeProcessLauncher();
        var launcher = new GroupLauncher(fake);
        var group = GroupWith(LaunchMode.Parallel, 0,
            new AppEntry { Target = @"C:\a.exe", Enabled = true },
            new AppEntry { Target = @"C:\b.exe", Enabled = false },
            new AppEntry { Target = @"C:\c.exe", Enabled = true });

        var result = await launcher.LaunchAsync(group);

        await Assert.That(result.Launched).IsEqualTo(2);
        await Assert.That(fake.Launched.Select(r => r.FileName)).IsEquivalentTo(new[] { @"C:\a.exe", @"C:\c.exe" });
    }

    [Test]
    public async Task Sequential_launches_in_order()
    {
        var fake = new FakeProcessLauncher();
        var launcher = new GroupLauncher(fake, _ => Task.CompletedTask);
        var group = GroupWith(LaunchMode.Sequential, 100,
            new AppEntry { Target = "1" },
            new AppEntry { Target = "2" },
            new AppEntry { Target = "3" });

        await launcher.LaunchAsync(group);

        await Assert.That(fake.Launched.Select(r => r.FileName).ToList())
            .IsEquivalentTo(new[] { "1", "2", "3" });
    }

    [Test]
    public async Task Sequential_waits_between_launches_only()
    {
        var fake = new FakeProcessLauncher();
        var delays = new List<int>();
        var launcher = new GroupLauncher(fake, ms => { delays.Add(ms); return Task.CompletedTask; });
        var group = GroupWith(LaunchMode.Sequential, 500,
            new AppEntry { Target = "1" },
            new AppEntry { Target = "2" },
            new AppEntry { Target = "3" });

        await launcher.LaunchAsync(group);

        // n apps => n-1 gaps
        await Assert.That(delays).IsEquivalentTo(new[] { 500, 500 });
    }

    [Test]
    public async Task Parallel_does_not_delay()
    {
        var fake = new FakeProcessLauncher();
        var delays = new List<int>();
        var launcher = new GroupLauncher(fake, ms => { delays.Add(ms); return Task.CompletedTask; });
        var group = GroupWith(LaunchMode.Parallel, 500,
            new AppEntry { Target = "1" },
            new AppEntry { Target = "2" });

        await launcher.LaunchAsync(group);

        await Assert.That(delays.Count).IsEqualTo(0);
    }

    [Test]
    public async Task One_failure_does_not_stop_the_rest()
    {
        var fake = new FakeProcessLauncher();
        fake.FailFor.Add(@"C:\bad.exe");
        var launcher = new GroupLauncher(fake);
        var group = GroupWith(LaunchMode.Parallel, 0,
            new AppEntry { Target = @"C:\good1.exe" },
            new AppEntry { Target = @"C:\bad.exe" },
            new AppEntry { Target = @"C:\good2.exe" });

        var result = await launcher.LaunchAsync(group);

        await Assert.That(result.Launched).IsEqualTo(2);
        await Assert.That(result.Failures.Count).IsEqualTo(1);
        await Assert.That(result.Failures[0].Entry.Target).IsEqualTo(@"C:\bad.exe");
    }

    [Test]
    public async Task Builds_request_from_entry_fields()
    {
        var entry = new AppEntry
        {
            Target = @"C:\tools\foo.exe",
            Arguments = "--flag",
            RunAsAdmin = true,
        };

        var request = GroupLauncher.BuildRequest(entry);

        await Assert.That(request.FileName).IsEqualTo(@"C:\tools\foo.exe");
        await Assert.That(request.Arguments).IsEqualTo("--flag");
        await Assert.That(request.WorkingDirectory).IsEqualTo(@"C:\tools");
        await Assert.That(request.RunAsAdmin).IsTrue();
    }

    [Test]
    public async Task Empty_group_launches_nothing()
    {
        var fake = new FakeProcessLauncher();
        var launcher = new GroupLauncher(fake);

        var result = await launcher.LaunchAsync(new AppGroup());

        await Assert.That(result.Launched).IsEqualTo(0);
        await Assert.That(result.Failures.Count).IsEqualTo(0);
    }
}
