using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd.Tests;

public class ProcessLauncherTests
{
    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Empty_target_throws_a_clear_error(string target)
    {
        var launcher = new ProcessLauncher();
        var request = new LaunchRequest(target, null, null, false);

        await Assert.That(() => launcher.Launch(request)).Throws<InvalidOperationException>();
    }
}
