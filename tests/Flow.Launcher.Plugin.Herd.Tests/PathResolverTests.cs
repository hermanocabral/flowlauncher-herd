using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd.Tests;

public class PathResolverTests
{
    [Test]
    public async Task Expand_resolves_environment_variables()
    {
        var expanded = PathResolver.Expand(@"%SystemRoot%\System32");

        await Assert.That(expanded).DoesNotContain("%");
        await Assert.That(expanded.EndsWith(@"\System32", StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task Expand_handles_null()
    {
        await Assert.That(PathResolver.Expand(null)).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task IsUrl_detects_scheme()
    {
        await Assert.That(PathResolver.IsUrl("https://example.com")).IsTrue();
        await Assert.That(PathResolver.IsUrl(@"C:\tools\foo.exe")).IsFalse();
    }

    [Test]
    public async Task ResolveWorkingDirectory_uses_explicit_value_expanded()
    {
        var entry = new AppEntry { Target = @"C:\tools\foo.exe", WorkingDirectory = @"%SystemRoot%" };

        var dir = PathResolver.ResolveWorkingDirectory(entry);

        await Assert.That(dir).IsEqualTo(Environment.GetEnvironmentVariable("SystemRoot"));
    }

    [Test]
    public async Task ResolveWorkingDirectory_defaults_to_executable_folder()
    {
        var entry = new AppEntry { Target = @"C:\tools\foo.exe" };

        var dir = PathResolver.ResolveWorkingDirectory(entry);

        await Assert.That(dir).IsEqualTo(@"C:\tools");
    }

    [Test]
    public async Task ResolveWorkingDirectory_is_null_for_url()
    {
        var entry = new AppEntry { Target = "https://example.com" };

        var dir = PathResolver.ResolveWorkingDirectory(entry);

        await Assert.That(dir).IsNull();
    }

    [Test]
    public async Task ResolveWorkingDirectory_is_null_for_bare_filename()
    {
        var entry = new AppEntry { Target = "notepad.exe" };

        var dir = PathResolver.ResolveWorkingDirectory(entry);

        await Assert.That(dir).IsNull();
    }
}
