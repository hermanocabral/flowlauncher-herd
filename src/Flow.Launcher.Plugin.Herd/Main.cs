using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Herd;

/// <summary>
/// Entry point Flow Launcher loads. Wiring is fleshed out in later tasks
/// (IAsyncPlugin query handling + ISettingProvider settings panel).
/// </summary>
public class Main : IPlugin
{
    private PluginInitContext _context = null!;

    public void Init(PluginInitContext context)
    {
        _context = context;
    }

    public List<Result> Query(Query query)
    {
        return new List<Result>();
    }
}
