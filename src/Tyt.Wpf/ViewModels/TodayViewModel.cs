using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
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

    [ObservableProperty]
    private ISeries[] _distributionSeries = [];

    public TodayViewModel(IUsageQuery usageQuery, ITrackingController tracking)
    {
        _usageQuery = usageQuery;
        _tracking = tracking;
        _tracking.StateChanged += (_, _) => Refresh();
        _usageQuery.Changed += (_, _) => Refresh();
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

        DistributionSeries = BuildDistribution(summary);

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

    private static readonly SKColor[] AppColors =
    [
        new(0x3B, 0x82, 0xF6),
        new(0x10, 0xB9, 0x81),
        new(0xF5, 0x9E, 0x0B),
        new(0x8B, 0x5C, 0xF6),
        new(0xEC, 0x48, 0x99),
        new(0x14, 0xB8, 0xA6)
    ];

    private static ISeries[] BuildDistribution(DaySummary summary)
    {
        var slices = new List<ISeries>();
        var colorIndex = 0;

        foreach (var app in summary.TopApps)
        {
            if (app.Active <= TimeSpan.Zero)
            {
                continue;
            }

            slices.Add(CreateSlice(
                app.App.Name,
                app.Active,
                AppColors[colorIndex % AppColors.Length]));
            colorIndex++;
        }

        if (summary.Idle > TimeSpan.Zero)
        {
            slices.Add(CreateSlice("Idle", summary.Idle, new SKColor(0x9C, 0xA3, 0xAF)));
        }

        return [.. slices];
    }

    private static PieSeries<double> CreateSlice(string name, TimeSpan duration, SKColor color)
    {
        var minutes = duration.TotalMinutes;
        return new PieSeries<double>
        {
            Name = name,
            Values = [minutes],
            InnerRadius = 50,
            Fill = new SolidColorPaint(color),
            ToolTipLabelFormatter = point =>
                $"{name}: {DurationFormatter.ToReadable(TimeSpan.FromMinutes(point.Coordinate.PrimaryValue))}"
        };
    }
}
