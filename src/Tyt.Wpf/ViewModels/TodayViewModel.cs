using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tyt.App.Dashboard;
using Tyt.Core.Abstractions;
using Tyt.Core.Models;
using Tyt.Wpf.Formatting;

namespace Tyt.Wpf.ViewModels;

public partial class TodayViewModel : ObservableObject
{
    private readonly IUsageQuery _usageQuery;
    private readonly ITrackingController _tracking;
    private readonly UsageAggregator _aggregator = new();

    [ObservableProperty]
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

    [ObservableProperty]
    private string _dateLabel = "";

    [ObservableProperty]
    private string _activeTotal = "";

    [ObservableProperty]
    private string _idleTotal = "";

    [ObservableProperty]
    private bool _hasData;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private bool _canGoNext;

    public ObservableCollection<AppUsageRow> TopApps { get; } = [];

    public ObservableCollection<SegmentRow> Segments { get; } = [];

    public TodayViewModel(IUsageQuery usageQuery, ITrackingController tracking)
    {
        _usageQuery = usageQuery;
        _tracking = tracking;
        _tracking.StateChanged += (_, _) => Refresh();
        Refresh();
    }

    [RelayCommand]
    private void PreviousDay()
    {
        SelectedDate = SelectedDate.AddDays(-1);
        Refresh();
    }

    [RelayCommand]
    private void NextDay()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (SelectedDate >= today)
        {
            return;
        }

        SelectedDate = SelectedDate.AddDays(1);
        Refresh();
    }

    public void Refresh()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        CanGoNext = SelectedDate < today;
        DateLabel = SelectedDate == today ? "Today" : SelectedDate.ToString("dddd, d MMMM yyyy");
        IsPaused = _tracking.IsPaused;

        var now = DateTime.Now;
        var segments = _usageQuery.GetDay(SelectedDate);
        var summary = _aggregator.SummarizeDay(segments, SelectedDate, now);

        HasData = summary.HasData;
        ActiveTotal = DurationFormatter.ToReadable(summary.Active);
        IdleTotal = DurationFormatter.ToReadable(summary.Idle);

        TopApps.Clear();
        foreach (var app in summary.TopApps)
        {
            TopApps.Add(new AppUsageRow
            {
                Name = app.App.Name,
                Duration = DurationFormatter.ToReadable(app.Active)
            });
        }

        Segments.Clear();
        foreach (var segment in segments)
        {
            var end = segment.EndedAt ?? now;
            var duration = UsageAggregator.Overlap(
                segment,
                SelectedDate.ToDateTime(TimeOnly.MinValue),
                SelectedDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
                now);

            Segments.Add(new SegmentRow
            {
                Title = segment.Kind == SegmentKind.Idle ? "Idle" : segment.App?.Name ?? "Unknown app",
                Kind = segment.Kind.ToString(),
                Range = segment.EndedAt is null
                    ? $"{segment.StartedAt:HH:mm} – now"
                    : $"{segment.StartedAt:HH:mm} – {end:HH:mm}",
                Duration = DurationFormatter.ToReadable(duration)
            });
        }
    }
}
