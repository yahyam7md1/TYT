using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tyt.App.Dashboard;
using Tyt.Core.Abstractions;
using Tyt.Core.Models;
using Tyt.Wpf.Formatting;

namespace Tyt.Wpf.ViewModels;

public partial class TimelineViewModel : ObservableObject
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
    private bool _hasData;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private bool _canGoNext;

    [ObservableProperty]
    private TimelineBlock? _selectedBlock;

    public ObservableCollection<TimelineBlock> Blocks { get; } = [];

    public TimelineViewModel(IUsageQuery usageQuery, ITrackingController tracking)
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

    [RelayCommand]
    private void SelectBlock(TimelineBlock? block) => SelectedBlock = block;

    public void Refresh()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        CanGoNext = SelectedDate < today;
        DateLabel = SelectedDate == today ? "Today" : SelectedDate.ToString("dddd, d MMMM yyyy");
        IsPaused = _tracking.IsPaused;

        var now = DateTime.Now;
        var dayStart = SelectedDate.ToDateTime(TimeOnly.MinValue);
        var dayEnd = SelectedDate.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var segments = _usageQuery.GetDay(SelectedDate);
        var summary = _aggregator.SummarizeDay(segments, SelectedDate, now);

        HasData = summary.HasData;
        ActiveTotal = DurationFormatter.ToReadable(summary.Active);
        SelectedBlock = null;

        Blocks.Clear();
        foreach (var segment in segments)
        {
            var duration = UsageAggregator.Overlap(segment, dayStart, dayEnd, now);
            if (duration <= TimeSpan.Zero)
            {
                continue;
            }

            var end = segment.EndedAt ?? now;
            var isIdle = segment.Kind == SegmentKind.Idle;
            var isOpen = segment.EndedAt is null;

            Blocks.Add(new TimelineBlock
            {
                Title = isIdle ? "Idle" : segment.App?.Name ?? "Unknown app",
                KindLabel = isIdle ? "Idle" : "Active",
                Range = isOpen
                    ? $"{segment.StartedAt:HH:mm} – now"
                    : $"{segment.StartedAt:HH:mm} – {end:HH:mm}",
                Duration = DurationFormatter.ToReadable(duration),
                IsIdle = isIdle,
                IsOpen = isOpen,
                BarHeight = Math.Clamp(duration.TotalMinutes * 1.2, 36, 160)
            });
        }
    }
}
