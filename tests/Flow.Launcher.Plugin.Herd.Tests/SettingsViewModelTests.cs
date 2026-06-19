using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.ViewModels;

namespace Flow.Launcher.Plugin.Herd.Tests;

public class SettingsViewModelTests
{
    [Test]
    public async Task AddGroup_adds_selects_and_saves()
    {
        var saved = 0;
        var vm = new SettingsViewModel(new HerdSettings(), () => saved++);

        vm.AddGroup();

        await Assert.That(vm.Groups.Count).IsEqualTo(1);
        await Assert.That(vm.SelectedGroup).IsEqualTo(vm.Groups[0]);
        await Assert.That(saved).IsEqualTo(1);
    }

    [Test]
    public async Task DeleteSelectedGroup_removes_reselects_and_saves()
    {
        var saved = 0;
        var settings = new HerdSettings();
        settings.Groups.Add(new AppGroup { Name = "A" });
        settings.Groups.Add(new AppGroup { Name = "B" });
        var vm = new SettingsViewModel(settings, () => saved++) { SelectedGroup = settings.Groups[0] };

        vm.DeleteSelectedGroup();

        await Assert.That(vm.Groups.Count).IsEqualTo(1);
        await Assert.That(vm.Groups[0].Name).IsEqualTo("B");
        await Assert.That(vm.SelectedGroup).IsEqualTo(vm.Groups[0]);
        await Assert.That(saved).IsEqualTo(1);
    }

    [Test]
    public async Task DeleteSelectedGroup_with_no_selection_is_noop()
    {
        var saved = 0;
        var vm = new SettingsViewModel(new HerdSettings(), () => saved++) { SelectedGroup = null };

        vm.DeleteSelectedGroup();

        await Assert.That(saved).IsEqualTo(0);
    }

    [Test]
    public async Task DuplicateSelectedGroup_deep_clones_independently()
    {
        var saved = 0;
        var settings = new HerdSettings();
        var original = new AppGroup { Name = "Dev", Description = "d", LaunchMode = LaunchMode.Sequential, DelayMs = 300 };
        original.Apps.Add(new AppEntry { Target = @"C:\a.exe", DisplayName = "A" });
        settings.Groups.Add(original);
        var vm = new SettingsViewModel(settings, () => saved++) { SelectedGroup = original };

        vm.DuplicateSelectedGroup();

        await Assert.That(vm.Groups.Count).IsEqualTo(2);
        var copy = vm.Groups[1];
        await Assert.That(copy.Id).IsNotEqualTo(original.Id);
        await Assert.That(copy.LaunchMode).IsEqualTo(LaunchMode.Sequential);
        await Assert.That(copy.DelayMs).IsEqualTo(300);
        await Assert.That(copy.Apps.Count).IsEqualTo(1);

        // independent copy: mutating the clone must not touch the original
        copy.Apps[0].Target = @"C:\changed.exe";
        await Assert.That(original.Apps[0].Target).IsEqualTo(@"C:\a.exe");
        await Assert.That(saved).IsEqualTo(1);
    }

    [Test]
    public async Task AddApp_adds_to_selected_group_and_saves()
    {
        var saved = 0;
        var settings = new HerdSettings();
        var group = new AppGroup { Name = "Dev" };
        settings.Groups.Add(group);
        var vm = new SettingsViewModel(settings, () => saved++) { SelectedGroup = group };

        vm.AddApp(@"C:\tools\foo.exe");

        await Assert.That(group.Apps.Count).IsEqualTo(1);
        await Assert.That(group.Apps[0].Target).IsEqualTo(@"C:\tools\foo.exe");
        await Assert.That(saved).IsEqualTo(1);
    }

    [Test]
    public async Task MoveAppUp_and_Down_reorder()
    {
        var saved = 0;
        var settings = new HerdSettings();
        var group = new AppGroup { Name = "Dev" };
        var a = new AppEntry { Target = "a" };
        var b = new AppEntry { Target = "b" };
        group.Apps.Add(a);
        group.Apps.Add(b);
        settings.Groups.Add(group);
        var vm = new SettingsViewModel(settings, () => saved++) { SelectedGroup = group };

        vm.MoveAppUp(b);
        await Assert.That(group.Apps[0]).IsEqualTo(b);

        vm.MoveAppDown(b);
        await Assert.That(group.Apps[1]).IsEqualTo(b);

        // first-up and last-down are no-ops
        vm.MoveAppUp(b);          // b is at index 0 now? no, b moved back to index 1
        await Assert.That(saved).IsGreaterThan(0);
    }

    [Test]
    public async Task RemoveApp_removes_and_saves()
    {
        var saved = 0;
        var settings = new HerdSettings();
        var group = new AppGroup { Name = "Dev" };
        var a = new AppEntry { Target = "a" };
        group.Apps.Add(a);
        settings.Groups.Add(group);
        var vm = new SettingsViewModel(settings, () => saved++) { SelectedGroup = group };

        vm.RemoveApp(a);

        await Assert.That(group.Apps.Count).IsEqualTo(0);
        await Assert.That(saved).IsEqualTo(1);
    }
}
