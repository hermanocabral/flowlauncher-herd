using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.ViewModels;
using Flow.Launcher.Plugin.Herd.Views;

namespace Flow.Launcher.Plugin.Herd.Tests;

/// <summary>
/// Loads the settings control's XAML on an STA thread to catch runtime markup errors
/// (bad resources, namespaces, bindings) that a compile alone won't surface.
/// </summary>
public class SettingsControlSmokeTests
{
    [Test]
    public async Task Control_loads_xaml_without_errors()
    {
        Exception? error = null;

        var thread = new Thread(() =>
        {
            try
            {
                var settings = new HerdSettings();
                settings.Groups.Add(new AppGroup { Name = "Sample", LaunchMode = LaunchMode.Sequential });
                var vm = new SettingsViewModel(settings, () => { });
                _ = new SettingsControl(vm);
            }
            catch (Exception ex)
            {
                error = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        await Assert.That(error).IsNull();
    }
}
