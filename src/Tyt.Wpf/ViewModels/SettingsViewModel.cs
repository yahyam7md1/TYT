using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tyt.Core.Abstractions;
using Tyt.Core.Models;

namespace Tyt.Wpf.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsStore _settings;
    private readonly ITrackingController _tracking;
    private readonly IActivityCommands _activity;
    private bool _loading;

    [ObservableProperty]
    private int _idleMinutes;

    [ObservableProperty]
    private bool _launchAtStartup;

    [ObservableProperty]
    private bool _isPaused;

    public SettingsViewModel(
        ISettingsStore settings,
        ITrackingController tracking,
        IActivityCommands activity)
    {
        _settings = settings;
        _tracking = tracking;
        _activity = activity;
        _tracking.StateChanged += (_, _) =>
        {
            IsPaused = _tracking.IsPaused;
        };

        _loading = true;
        var current = _settings.Get();
        IdleMinutes = Math.Clamp((int)Math.Round(current.IdleThreshold.TotalMinutes), 1, 60);
        LaunchAtStartup = current.LaunchAtStartup;
        IsPaused = _tracking.IsPaused;
        _loading = false;
    }

    partial void OnIdleMinutesChanged(int value)
    {
        if (_loading)
        {
            return;
        }

        Persist();
    }

    partial void OnLaunchAtStartupChanged(bool value)
    {
        if (_loading)
        {
            return;
        }

        Persist();
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (_tracking.IsPaused)
        {
            _tracking.Resume();
        }
        else
        {
            _tracking.Pause();
        }
    }

    [RelayCommand]
    private void DeleteAllData()
    {
        var result = System.Windows.MessageBox.Show(
            "Delete all locally stored activity records? This cannot be undone (mock data in this build).",
            "Delete activity data",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            _activity.DeleteAll();
        }
    }

    private void Persist()
    {
        _settings.Save(new TrackingSettings
        {
            IdleThreshold = TimeSpan.FromMinutes(Math.Clamp(IdleMinutes, 1, 60)),
            LaunchAtStartup = LaunchAtStartup
        });
    }
}
