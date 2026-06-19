using System.Text.Json;
using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.Tests;

public class AppEntryTests
{
    [Test]
    public async Task New_entry_is_enabled_by_default()
    {
        var entry = new AppEntry();
        await Assert.That(entry.Enabled).IsTrue();
    }

    [Test]
    public async Task DisplayLabel_uses_DisplayName_when_set()
    {
        var entry = new AppEntry { Target = @"C:\tools\foo.exe", DisplayName = "My Tool" };
        await Assert.That(entry.DisplayLabel).IsEqualTo("My Tool");
    }

    [Test]
    public async Task DisplayLabel_falls_back_to_target_filename()
    {
        var entry = new AppEntry { Target = @"C:\tools\foo.exe" };
        await Assert.That(entry.DisplayLabel).IsEqualTo("foo");
    }

    [Test]
    public async Task DisplayLabel_keeps_full_url()
    {
        var entry = new AppEntry { Target = "https://example.com/path" };
        await Assert.That(entry.DisplayLabel).IsEqualTo("https://example.com/path");
    }

    [Test]
    public async Task Setting_property_raises_PropertyChanged()
    {
        var entry = new AppEntry();
        var raised = new List<string?>();
        entry.PropertyChanged += (_, args) => raised.Add(args.PropertyName);

        entry.Target = "x";

        await Assert.That(raised).Contains(nameof(AppEntry.Target));
    }

    [Test]
    public async Task Setting_Target_also_raises_DisplayLabel_change()
    {
        var entry = new AppEntry();
        var raised = new List<string?>();
        entry.PropertyChanged += (_, args) => raised.Add(args.PropertyName);

        entry.Target = @"C:\tools\foo.exe";

        await Assert.That(raised).Contains(nameof(AppEntry.DisplayLabel));
    }
}

public class AppGroupTests
{
    [Test]
    public async Task New_group_has_unique_id()
    {
        var group = new AppGroup();
        await Assert.That(group.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task New_group_defaults_to_parallel_with_empty_apps()
    {
        var group = new AppGroup();
        await Assert.That(group.LaunchMode).IsEqualTo(LaunchMode.Parallel);
        await Assert.That(group.Apps).IsNotNull();
        await Assert.That(group.Apps.Count).IsEqualTo(0);
    }

    [Test]
    public async Task EnabledApps_excludes_disabled_entries()
    {
        var group = new AppGroup();
        group.Apps.Add(new AppEntry { Target = "a", Enabled = true });
        group.Apps.Add(new AppEntry { Target = "b", Enabled = false });

        var enabled = group.EnabledApps.ToList();

        await Assert.That(enabled.Count).IsEqualTo(1);
        await Assert.That(enabled[0].Target).IsEqualTo("a");
    }
}

public class HerdSettingsTests
{
    [Test]
    public async Task New_settings_has_empty_groups()
    {
        var settings = new HerdSettings();
        await Assert.That(settings.Groups).IsNotNull();
        await Assert.That(settings.Groups.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Json_round_trip_preserves_groups_and_apps()
    {
        var settings = new HerdSettings();
        var group = new AppGroup
        {
            Name = "Dev",
            Description = "Editor + browser",
            LaunchMode = LaunchMode.Sequential,
            DelayMs = 500,
        };
        group.Apps.Add(new AppEntry
        {
            Target = @"C:\tools\code.exe",
            DisplayName = "VS Code",
            Arguments = "--new-window",
            WorkingDirectory = @"C:\projects",
            RunAsAdmin = true,
            Enabled = true,
        });
        group.Apps.Add(new AppEntry { Target = "https://github.com", Enabled = false });
        settings.Groups.Add(group);

        var json = JsonSerializer.Serialize(settings);
        var restored = JsonSerializer.Deserialize<HerdSettings>(json)!;

        await Assert.That(restored.Groups.Count).IsEqualTo(1);
        var rg = restored.Groups[0];
        await Assert.That(rg.Id).IsEqualTo(group.Id);
        await Assert.That(rg.Name).IsEqualTo("Dev");
        await Assert.That(rg.LaunchMode).IsEqualTo(LaunchMode.Sequential);
        await Assert.That(rg.DelayMs).IsEqualTo(500);
        await Assert.That(rg.Apps.Count).IsEqualTo(2);
        await Assert.That(rg.Apps[0].DisplayName).IsEqualTo("VS Code");
        await Assert.That(rg.Apps[0].RunAsAdmin).IsTrue();
        await Assert.That(rg.Apps[1].Enabled).IsFalse();
    }

    [Test]
    public async Task LaunchMode_serializes_as_readable_string()
    {
        var settings = new HerdSettings();
        settings.Groups.Add(new AppGroup { Name = "G", LaunchMode = LaunchMode.Sequential });

        var json = JsonSerializer.Serialize(settings);

        await Assert.That(json).Contains("Sequential");
    }
}
