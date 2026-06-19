using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.Services;

/// <summary>
/// Turns a search term into Flow <see cref="Result"/>s for matching groups. The fuzzy
/// scorer and the launch action are injected so the service has no dependency on the
/// live Flow context and is fully unit-testable.
/// </summary>
public sealed class QueryService
{
    private readonly Func<string, string, int> _score;
    private readonly Func<AppGroup, bool> _launch;
    private readonly string _defaultIcon;

    public QueryService(Func<string, string, int> score, Func<AppGroup, bool> launch, string defaultIcon)
    {
        _score = score;
        _launch = launch;
        _defaultIcon = defaultIcon;
    }

    /// <summary>Builds results for the groups that match <paramref name="search"/> (all of them when blank).</summary>
    public List<Result> Query(string search, IReadOnlyList<AppGroup> groups)
    {
        var term = search?.Trim() ?? string.Empty;

        IEnumerable<(AppGroup group, int score)> matched = term.Length == 0
            ? groups.Select(group => (group, 0))
            : groups
                .Select(group => (group, score: ScoreGroup(term, group)))
                .Where(x => x.score > 0)
                .OrderByDescending(x => x.score);

        return matched.Select(x => ToResult(x.group, x.score)).ToList();
    }

    /// <summary>A single result shown when no groups exist yet; its action opens settings.</summary>
    public static List<Result> NoGroupsHint(string defaultIcon, Action openSettings) => new()
    {
        new Result
        {
            Title = "No app groups yet",
            SubTitle = "Press Enter to open Herd settings and create your first group",
            IcoPath = defaultIcon,
            Action = _ =>
            {
                openSettings();
                return true;
            },
        },
    };

    private int ScoreGroup(string term, AppGroup group)
    {
        var score = _score(term, group.Name);
        if (!string.IsNullOrEmpty(group.Description))
        {
            score = Math.Max(score, _score(term, group.Description));
        }

        return score;
    }

    private Result ToResult(AppGroup group, int score)
    {
        return new Result
        {
            Title = group.Name,
            SubTitle = BuildSubTitle(group),
            IcoPath = string.IsNullOrWhiteSpace(group.IconPath) ? _defaultIcon : group.IconPath,
            Score = score,
            ContextData = group,
            AutoCompleteText = group.Name,
            TitleToolTip = group.Name,
            SubTitleToolTip = BuildAppList(group),
            Action = _ => _launch(group),
        };
    }

    private static string BuildSubTitle(AppGroup group) =>
        !string.IsNullOrWhiteSpace(group.Description) ? group.Description! : BuildAppSummary(group);

    private static string BuildAppSummary(AppGroup group)
    {
        var count = group.Apps.Count;
        if (count == 0)
        {
            return "No apps yet — open settings to add some.";
        }

        var labels = string.Join(", ", group.Apps.Select(a => a.DisplayLabel));
        return $"{count} app{(count == 1 ? string.Empty : "s")}: {labels}";
    }

    private static string BuildAppList(AppGroup group) =>
        group.Apps.Count == 0
            ? "No apps configured."
            : string.Join(Environment.NewLine, group.Apps.Select(a =>
                a.Enabled ? $"• {a.DisplayLabel}" : $"• {a.DisplayLabel} (disabled)"));
}
