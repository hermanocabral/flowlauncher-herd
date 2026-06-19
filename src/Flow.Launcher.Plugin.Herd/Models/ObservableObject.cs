using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Flow.Launcher.Plugin.Herd.Models;

/// <summary>
/// Minimal <see cref="INotifyPropertyChanged"/> base so models can drive WPF
/// two-way bindings without pulling in an MVVM framework.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>Sets <paramref name="field"/> and raises change notifications when the value differs.</summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
