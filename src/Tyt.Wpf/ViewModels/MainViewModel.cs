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

    public PlaceholderViewModel Week { get; } = new(
        "Week",
        "Weekly totals and per-day bars will land in Phase 3. Same mock segments as Today.");

    public PlaceholderViewModel Timeline { get; } = new(
        "Timeline",
        "A chronological day view will land in Phase 4.");

    public PlaceholderViewModel Settings { get; } = new(
        "Settings",
        "Idle threshold, pause, autostart, and delete data will land in Phase 5.");

    public MainViewModel(IUsageQuery usageQuery, ITrackingController tracking, ISettingsStore settingsStore)
    {
        _ = settingsStore;
        _tracking = tracking;
        Today = new TodayViewModel(usageQuery, tracking);
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
    }

    [RelayCommand]
    private void GoTimeline()
    {
        SelectedNav = "Timeline";
        CurrentPage = Timeline;
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
