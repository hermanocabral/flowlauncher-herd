using System.Windows.Controls;
using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;
using Flow.Launcher.Plugin.Herd.ViewModels;
using Flow.Launcher.Plugin.Herd.Views;

namespace Flow.Launcher.Plugin.Herd;

/// <summary>
/// Composition root Flow Launcher loads. Wires the tested services together and forwards
/// queries/launches; all behaviour lives in <see cref="QueryService"/> and
/// <see cref="GroupLauncher"/>.
/// </summary>
public class Main : IAsyncPlugin, ISettingProvider
{
    internal const string DefaultIcon = @"Images\icon.png";

    private PluginInitContext _context = null!;
    private HerdSettings _settings = null!;
    private GroupLauncher _launcher = null!;
    private QueryService _query = null!;

    public Task InitAsync(PluginInitContext context)
    {
        _context = context;
        // Load exactly once: Flow's storage rebinds its Data object on each Load, so a second
        // load would desync this instance from the one the settings panel edits and queries read.
        _settings = context.API.LoadSettingJsonStorage<HerdSettings>();
        _launcher = new GroupLauncher(new ProcessLauncher());
        _query = new QueryService(
            (query, text) => _context.API.FuzzySearch(query, text).Score,
            LaunchGroup,
            DefaultIcon);

        return Task.CompletedTask;
    }

    public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        if (_settings.Groups.Count == 0)
        {
            return Task.FromResult(
                QueryService.NoGroupsHint(DefaultIcon, () => _context.API.OpenSettingDialog()));
        }

        return Task.FromResult(_query.Query(query.Search, _settings.Groups));
    }

    public Control CreateSettingPanel()
    {
        var vm = new SettingsViewModel(_settings, () => _context.API.SaveSettingJsonStorage<HerdSettings>());
        return new SettingsControl(vm);
    }

    private bool LaunchGroup(AppGroup group)
    {
        // Run off the UI thread: a synchronous shell launch (e.g. a blocking UAC prompt) must
        // not freeze Flow, and sequential delays must not block it. Returning true hides Flow now.
        _ = Task.Run(() => RunLaunchAsync(group));
        return true;
    }

    private async Task RunLaunchAsync(AppGroup group)
    {
        try
        {
            var result = await _launcher.LaunchAsync(group);
            if (result.Failures.Count > 0)
            {
                var names = string.Join(", ", result.Failures.Select(f => f.Entry.DisplayLabel));
                _context.API.ShowMsgError("Herd — some apps didn't start", names);
            }
        }
        catch (Exception ex)
        {
            _context.API.ShowMsgError("Herd — launch failed", ex.Message);
        }
    }
}
