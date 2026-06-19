using System.Collections.ObjectModel;
using Flow.Launcher.Plugin.Herd.Models;

namespace Flow.Launcher.Plugin.Herd.ViewModels;

/// <summary>
/// Drives the settings panel: group selection plus add/delete/duplicate and per-app
/// add/remove/reorder. Persists through an injected save callback after each change,
/// so the logic is testable without WPF.
/// </summary>
public class SettingsViewModel : ObservableObject
{
    private readonly Action _save;
    private AppGroup? _selectedGroup;

    public SettingsViewModel(HerdSettings settings, Action save)
    {
        Settings = settings;
        _save = save;
        _selectedGroup = settings.Groups.FirstOrDefault();
    }

    public HerdSettings Settings { get; }

    public ObservableCollection<AppGroup> Groups => Settings.Groups;

    public AppGroup? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (SetProperty(ref _selectedGroup, value))
            {
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    /// <summary>True when a group is selected (drives detail-pane visibility).</summary>
    public bool HasSelection => SelectedGroup is not null;

    public void AddGroup()
    {
        var group = new AppGroup { Name = "New group" };
        Groups.Add(group);
        SelectedGroup = group;
        Save();
    }

    public void DeleteSelectedGroup()
    {
        if (SelectedGroup is null)
        {
            return;
        }

        var index = Groups.IndexOf(SelectedGroup);
        Groups.Remove(SelectedGroup);
        SelectedGroup = Groups.Count == 0 ? null : Groups[Math.Min(index, Groups.Count - 1)];
        Save();
    }

    public void DuplicateSelectedGroup()
    {
        if (SelectedGroup is null)
        {
            return;
        }

        var copy = SelectedGroup.Clone();
        copy.Name = $"{SelectedGroup.Name} (copy)";
        Groups.Add(copy);
        SelectedGroup = copy;
        Save();
    }

    public AppEntry? AddApp(string target)
    {
        if (SelectedGroup is null)
        {
            return null;
        }

        var entry = new AppEntry { Target = target };
        SelectedGroup.Apps.Add(entry);
        Save();
        return entry;
    }

    public void RemoveApp(AppEntry entry)
    {
        if (SelectedGroup?.Apps.Remove(entry) == true)
        {
            Save();
        }
    }

    public void MoveAppUp(AppEntry entry) => MoveApp(entry, -1);

    public void MoveAppDown(AppEntry entry) => MoveApp(entry, +1);

    private void MoveApp(AppEntry entry, int direction)
    {
        var apps = SelectedGroup?.Apps;
        if (apps is null)
        {
            return;
        }

        var from = apps.IndexOf(entry);
        var to = from + direction;
        if (from < 0 || to < 0 || to >= apps.Count)
        {
            return;
        }

        apps.Move(from, to);
        Save();
    }

    public void Save() => _save();
}
