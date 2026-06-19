using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd;

/// <summary>
/// Composition root Flow Launcher loads. Wires the tested services together and forwards
/// queries/launches; all behaviour lives in <see cref="QueryService"/> and
/// <see cref="GroupLauncher"/>.
/// </summary>
public class Main : IAsyncPlugin
{
    internal const string DefaultIcon = @"Images\icon.png";

    private PluginInitContext _context = null!;
    private HerdSettings _settings = null!;
    private GroupLauncher _launcher = null!;
    private QueryService _query = null!;

    public Task InitAsync(PluginInitContext context)
    {
        _context = context;
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

    private bool LaunchGroup(AppGroup group)
    {
        // Fire-and-forget so the Flow window hides immediately; sequential delays must not block it.
        _ = RunLaunchAsync(group);
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
