using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.Services;

namespace Flow.Launcher.Plugin.Herd.Tests;

public class QueryServiceTests
{
    private const string DefaultIcon = @"Images\icon.png";

    // Simple case-insensitive substring scorer standing in for Flow's FuzzySearch.
    private static int Substring(string query, string text) =>
        !string.IsNullOrEmpty(text) && text.Contains(query, StringComparison.OrdinalIgnoreCase) ? 100 : 0;

    private static QueryService NewService(out List<AppGroup> launched, Func<string, string, int>? scorer = null)
    {
        var launchedLocal = new List<AppGroup>();
        launched = launchedLocal;
        return new QueryService(
            scorer ?? Substring,
            group => { launchedLocal.Add(group); return true; },
            DefaultIcon);
    }

    private static AppGroup Group(string name, string? description = null, params string[] targets)
    {
        var group = new AppGroup { Name = name, Description = description };
        foreach (var t in targets)
        {
            group.Apps.Add(new AppEntry { Target = t });
        }

        return group;
    }

    [Test]
    public async Task Empty_term_lists_all_groups()
    {
        var service = NewService(out _);
        var groups = new List<AppGroup> { Group("Dev"), Group("Media") };

        var results = service.Query("", groups);

        await Assert.That(results.Select(r => r.Title)).IsEquivalentTo(new[] { "Dev", "Media" });
    }

    [Test]
    public async Task Filters_by_name_match()
    {
        var service = NewService(out _);
        var groups = new List<AppGroup> { Group("Dev"), Group("Media") };

        var results = service.Query("dev", groups);

        await Assert.That(results.Count).IsEqualTo(1);
        await Assert.That(results[0].Title).IsEqualTo("Dev");
    }

    [Test]
    public async Task Matches_description_too()
    {
        var service = NewService(out _);
        var groups = new List<AppGroup> { Group("Dev", "coding setup"), Group("Media", "music and video") };

        var results = service.Query("music", groups);

        await Assert.That(results.Count).IsEqualTo(1);
        await Assert.That(results[0].Title).IsEqualTo("Media");
    }

    [Test]
    public async Task Non_matching_term_returns_empty()
    {
        var service = NewService(out _);
        var groups = new List<AppGroup> { Group("Dev"), Group("Media") };

        var results = service.Query("zzz", groups);

        await Assert.That(results.Count).IsEqualTo(0);
    }

    [Test]
    public async Task SubTitle_uses_description_when_present()
    {
        var service = NewService(out _);
        var groups = new List<AppGroup> { Group("Dev", "my coding setup") };

        var results = service.Query("", groups);

        await Assert.That(results[0].SubTitle).IsEqualTo("my coding setup");
    }

    [Test]
    public async Task SubTitle_falls_back_to_app_summary()
    {
        var service = NewService(out _);
        var groups = new List<AppGroup> { Group("Dev", null, @"C:\code.exe", @"C:\chrome.exe") };

        var results = service.Query("", groups);

        await Assert.That(results[0].SubTitle).Contains("code");
        await Assert.That(results[0].SubTitle).Contains("chrome");
    }

    [Test]
    public async Task IcoPath_uses_group_icon_else_default()
    {
        var service = NewService(out _);
        var custom = Group("Dev");
        custom.IconPath = @"C:\icons\dev.png";
        var groups = new List<AppGroup> { custom, Group("Media") };

        var results = service.Query("", groups);

        await Assert.That(results.Single(r => r.Title == "Dev").IcoPath).IsEqualTo(@"C:\icons\dev.png");
        await Assert.That(results.Single(r => r.Title == "Media").IcoPath).IsEqualTo(DefaultIcon);
    }

    [Test]
    public async Task Results_are_sorted_by_score_descending()
    {
        // "ed" scores Media higher than Dev via this custom scorer.
        int Scorer(string q, string t) => t == "Media" ? 90 : t == "Dev" ? 40 : 0;
        var service = NewService(out _, Scorer);
        var groups = new List<AppGroup> { Group("Dev"), Group("Media") };

        var results = service.Query("e", groups);

        await Assert.That(results.Select(r => r.Title).ToList()).IsEquivalentTo(new[] { "Media", "Dev" });
    }

    [Test]
    public async Task Action_launches_group_and_hides_window()
    {
        var service = NewService(out var launched);
        var groups = new List<AppGroup> { Group("Dev") };

        var results = service.Query("", groups);
        var hide = results[0].Action!(null!);

        await Assert.That(hide).IsTrue();
        await Assert.That(launched.Single().Name).IsEqualTo("Dev");
    }

    [Test]
    public async Task Result_autocompletes_to_group_name()
    {
        var service = NewService(out _);

        var results = service.Query("", new List<AppGroup> { Group("Dev") });

        await Assert.That(results[0].AutoCompleteText).IsEqualTo("Dev");
    }

    [Test]
    public async Task Preview_lists_apps_and_sequential_launch_summary()
    {
        var service = NewService(out _);
        var group = new AppGroup { Name = "Dev", LaunchMode = LaunchMode.Sequential, DelayMs = 200 };
        group.Apps.Add(new AppEntry { Target = @"C:\code.exe" });
        group.Apps.Add(new AppEntry { Target = @"C:\chrome.exe", RunAsAdmin = true });
        group.Apps.Add(new AppEntry { Target = @"C:\slack.exe", Enabled = false });

        var preview = service.Query("", new List<AppGroup> { group })[0].Preview;

        await Assert.That(preview).IsNotNull();
        await Assert.That(preview.Description).Contains("code");
        await Assert.That(preview.Description).Contains("chrome");
        await Assert.That(preview.Description).Contains("one by one");
        await Assert.That(preview.Description).Contains("200");
        await Assert.That(preview.Description).Contains("admin");
        await Assert.That(preview.Description).Contains("disabled");
    }

    [Test]
    public async Task Preview_says_all_at_once_for_parallel_groups()
    {
        var service = NewService(out _);
        var group = new AppGroup { Name = "Dev", LaunchMode = LaunchMode.Parallel };
        group.Apps.Add(new AppEntry { Target = @"C:\a.exe" });

        var preview = service.Query("", new List<AppGroup> { group })[0].Preview;

        await Assert.That(preview.Description).Contains("all at once");
    }

    [Test]
    public async Task NoGroupsHint_returns_single_result_that_opens_settings()
    {
        var opened = false;

        var results = QueryService.NoGroupsHint(DefaultIcon, () => opened = true);

        await Assert.That(results.Count).IsEqualTo(1);
        await Assert.That(results[0].IcoPath).IsEqualTo(DefaultIcon);

        var hide = results[0].Action!(null!);

        await Assert.That(hide).IsTrue();
        await Assert.That(opened).IsTrue();
    }
}
