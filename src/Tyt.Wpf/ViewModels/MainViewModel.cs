using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tyt.Core.Abstractions;

namespace Tyt.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITrackingController _tracking;

    [ObservableProperty]
    private object? _currentPage;

    [ObservableProperty]
    private string _selectedNav = "Today";

    [ObservableProperty]
    private bool _isPaused;

    public TodayViewModel Today { get; }

    public WeekViewModel Week { get; }

    public TimelineViewModel Timeline { get; }

    public SettingsViewModel Settings { get; }

    public MainViewModel(
        IUsageQuery usageQuery,
        ITrackingController tracking,
        ISettingsStore settingsStore,
        IActivityCommands activity)
    {
        _tracking = tracking;
        Today = new TodayViewModel(usageQuery, tracking);
        Week = new WeekViewModel(usageQuery, tracking);
        Timeline = new TimelineViewModel(usageQuery, tracking);
        Settings = new SettingsViewModel(settingsStore, tracking, activity);
        CurrentPage = Today;
        _tracking.StateChanged += (_, _) => UpdateStatus();
        UpdateStatus();
    }

    public string StatusText => IsPaused ? "Paused" : "Tracking";

    [RelayCommand]
    private void GoToday()
    {
        SelectedNav = "Today";
        CurrentPage = Today;
        Today.Refresh();
    }

    [RelayCommand]
    private void GoWeek()
    {
        SelectedNav = "Week";
        CurrentPage = Week;
        Week.Refresh();
    }

    [RelayCommand]
    private void GoTimeline()
    {
        SelectedNav = "Timeline";
        CurrentPage = Timeline;
        Timeline.Refresh();
    }

    [RelayCommand]
    private void GoSettings()
    {
        SelectedNav = "Settings";
        CurrentPage = Settings;
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

    private void UpdateStatus()
    {
        IsPaused = _tracking.IsPaused;
        OnPropertyChanged(nameof(StatusText));
    }
}
