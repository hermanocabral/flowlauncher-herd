using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.ViewModels;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.Herd.Views;

/// <summary>
/// WPF settings panel. The group list and per-app cards bind to the models; structural
/// edits go through <see cref="SettingsViewModel"/>. Advanced per-app fields live in an
/// expander so the common case stays simple. Text edits persist when the panel unloads.
/// </summary>
public partial class SettingsControl : UserControl
{
    private readonly SettingsViewModel _vm;

    public SettingsControl(SettingsViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        InitializeComponent();
        Unloaded += (_, _) => _vm.Save();
    }

    private static AppEntry? EntryOf(object sender) =>
        (sender as FrameworkElement)?.DataContext as AppEntry;

    private void AddGroup_Click(object sender, RoutedEventArgs e) => _vm.AddGroup();

    private void DuplicateGroup_Click(object sender, RoutedEventArgs e) => _vm.DuplicateSelectedGroup();

    private void DeleteGroup_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.SelectedGroup is null)
        {
            return;
        }

        var confirm = MessageBox.Show(
            $"Delete group \"{_vm.SelectedGroup.Name}\"?",
            "Herd",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm == MessageBoxResult.Yes)
        {
            _vm.DeleteSelectedGroup();
        }
    }

    private void AddApp_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: { } menu })
        {
            menu.PlacementTarget = (UIElement)sender;
            menu.IsOpen = true;
        }
    }

    private void AddFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Title = "Pick an application or file", CheckFileExists = true };
        if (dialog.ShowDialog() == true)
        {
            _vm.AddApp(dialog.FileName);
        }
    }

    private void AddFolder_Click(object sender, RoutedEventArgs e)
    {
        var path = PickFolder("Pick a folder to open");
        if (path is not null)
        {
            _vm.AddApp(path);
        }
    }

    private void AddUrl_Click(object sender, RoutedEventArgs e) => _vm.AddApp("https://");

    private void BrowseTarget_Click(object sender, RoutedEventArgs e)
    {
        if (EntryOf(sender) is not { } entry)
        {
            return;
        }

        var dialog = new OpenFileDialog { Title = "Pick an application or file", CheckFileExists = true };
        if (dialog.ShowDialog() == true)
        {
            entry.Target = dialog.FileName;
            _vm.Save();
        }
    }

    private void BrowseWorkingDir_Click(object sender, RoutedEventArgs e)
    {
        if (EntryOf(sender) is not { } entry)
        {
            return;
        }

        var path = PickFolder("Pick the working directory");
        if (path is not null)
        {
            entry.WorkingDirectory = path;
            _vm.Save();
        }
    }

    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        if (EntryOf(sender) is { } entry)
        {
            _vm.MoveAppUp(entry);
        }
    }

    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        if (EntryOf(sender) is { } entry)
        {
            _vm.MoveAppDown(entry);
        }
    }

    private void RemoveApp_Click(object sender, RoutedEventArgs e)
    {
        if (EntryOf(sender) is { } entry)
        {
            _vm.RemoveApp(entry);
        }
    }

    private void BrowseIcon_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.SelectedGroup is null)
        {
            return;
        }

        var dialog = new OpenFileDialog
        {
            Title = "Pick an icon",
            Filter = "Images and icons|*.png;*.ico;*.jpg;*.jpeg;*.bmp|All files|*.*",
        };
        if (dialog.ShowDialog() == true)
        {
            _vm.SelectedGroup.IconPath = dialog.FileName;
            _vm.Save();
        }
    }

    private void ClearIcon_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.SelectedGroup is not null)
        {
            _vm.SelectedGroup.IconPath = null;
            _vm.Save();
        }
    }

    private static string? PickFolder(string description)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = description,
            UseDescriptionForTitle = true,
        };

        return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK
            ? dialog.SelectedPath
            : null;
    }
}
