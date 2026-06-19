using System.IO;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Herd.Models;
using Flow.Launcher.Plugin.Herd.ViewModels;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.Herd.Views;

/// <summary>
/// WPF settings panel. Bindings keep the models in sync; structural edits go through
/// <see cref="SettingsViewModel"/>. Text edits are persisted when the panel unloads.
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

    private void AddUrl_Click(object sender, RoutedEventArgs e)
    {
        var entry = _vm.AddApp("https://");
        if (entry is not null)
        {
            AppsGrid.SelectedItem = entry;
        }
    }

    private void SetWorkingDir_Click(object sender, RoutedEventArgs e)
    {
        if (AppsGrid.SelectedItem is not AppEntry entry)
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
        if (AppsGrid.SelectedItem is AppEntry entry)
        {
            _vm.MoveAppUp(entry);
        }
    }

    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        if (AppsGrid.SelectedItem is AppEntry entry)
        {
            _vm.MoveAppDown(entry);
        }
    }

    private void RemoveApp_Click(object sender, RoutedEventArgs e)
    {
        if (AppsGrid.SelectedItem is AppEntry entry)
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
