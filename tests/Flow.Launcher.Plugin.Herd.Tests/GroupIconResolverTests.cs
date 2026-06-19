using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd.Tests;

public class GroupIconResolverTests
{
    private static AppGroup GroupWith(string? iconPath, params string[] targets)
    {
        var group = new AppGroup { IconPath = iconPath };
        foreach (var t in targets)
        {
            group.Apps.Add(new AppEntry { Target = t });
        }

        return group;
    }

    [Test]
    public async Task Uses_custom_icon_when_set_and_present()
    {
        var group = GroupWith(@"C:\icons\dev.png", @"C:\tools\foo.exe");

        var result = GroupIconResolver.Resolve(group, p => p == @"C:\icons\dev.png");

        await Assert.That(result.Kind).IsEqualTo(GroupIconKind.Custom);
        await Assert.That(result.Path).IsEqualTo(@"C:\icons\dev.png");
    }

    [Test]
    public async Task Falls_back_to_first_targets_icon()
    {
        var group = GroupWith(null, @"C:\tools\foo.exe");

        var result = GroupIconResolver.Resolve(group, p => p == @"C:\tools\foo.exe");

        await Assert.That(result.Kind).IsEqualTo(GroupIconKind.Target);
        await Assert.That(result.Path).IsEqualTo(@"C:\tools\foo.exe");
    }

    [Test]
    public async Task Missing_custom_icon_falls_through_to_target()
    {
        var group = GroupWith(@"C:\gone.png", @"C:\tools\foo.exe");

        var result = GroupIconResolver.Resolve(group, p => p == @"C:\tools\foo.exe");

        await Assert.That(result.Kind).IsEqualTo(GroupIconKind.Target);
    }

    [Test]
    public async Task Default_when_first_target_is_a_url()
    {
        var group = GroupWith(null, "https://example.com");

        var result = GroupIconResolver.Resolve(group, _ => true);

        await Assert.That(result.Kind).IsEqualTo(GroupIconKind.Default);
    }

    [Test]
    public async Task Default_when_no_apps_and_no_icon()
    {
        var result = GroupIconResolver.Resolve(new AppGroup(), _ => false);

        await Assert.That(result.Kind).IsEqualTo(GroupIconKind.Default);
    }
}
