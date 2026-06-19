namespace Flow.Launcher.Plugin.Herd.Tests;

/// <summary>
/// Confirms the TUnit / Microsoft.Testing.Platform harness runs and that the
/// test project links against the plugin assembly. Real coverage lands per feature.
/// </summary>
public class SmokeTests
{
    [Test]
    public async Task Harness_runs()
    {
        await Assert.That(1 + 1).IsEqualTo(2);
    }

    [Test]
    public async Task Plugin_assembly_is_referenced()
    {
        var plugin = new Herd.Main();
        await Assert.That(plugin).IsNotNull();
    }
}
