using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Tyt.App.Dashboard;
using Tyt.Core.Abstractions;
using Tyt.Wpf.Formatting;

namespace Tyt.Wpf.ViewModels;

public partial class WeekViewModel : ObservableObject
{
    private readonly IUsageQuery _usageQuery;
    private readonly ITrackingController _tracking;
    private readonly UsageAggregator _aggregator = new();

    [ObservableProperty]
    private DateOnly _weekStart = WeekCalendar.StartOfWeek(DateOnly.FromDateTime(DateTime.Today));

    [ObservableProperty]
    private string _rangeLabel = "";

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

    [ObservableProperty]
    private ISeries[] _dailySeries = [];

    [ObservableProperty]
    private Axis[] _xAxes = [];

    [ObservableProperty]
    private Axis[] _yAxes =
    [
        new Axis { MinLimit = 0, Name = "Hours" }
    ];

    public ObservableCollection<AppUsageRow> TopApps { get; } = [];

    public WeekViewModel(IUsageQuery usageQuery, ITrackingController tracking)
    {
        _usageQuery = usageQuery;
        _tracking = tracking;
        _tracking.StateChanged += (_, _) => Refresh();
        _usageQuery.Changed += (_, _) => Refresh();
        Refresh();
    }

    [RelayCommand]
    private void PreviousWeek()
    {
        WeekStart = WeekStart.AddDays(-7);
        Refresh();
    }

    [RelayCommand]
    private void NextWeek()
    {
        var currentWeek = WeekCalendar.StartOfWeek(DateOnly.FromDateTime(DateTime.Today));
        if (WeekStart >= currentWeek)
        {
            return;
        }

        WeekStart = WeekStart.AddDays(7);
        Refresh();
    }

    public void Refresh()
    {
        var currentWeek = WeekCalendar.StartOfWeek(DateOnly.FromDateTime(DateTime.Today));
        CanGoNext = WeekStart < currentWeek;
        var weekEnd = WeekCalendar.EndOfWeek(WeekStart);
        RangeLabel = $"{WeekStart:d MMM} – {weekEnd:d MMM yyyy}";
        IsPaused = _tracking.IsPaused;

        var now = DateTime.Now;
        var segments = _usageQuery.GetRange(WeekStart, weekEnd);
        var summary = _aggregator.SummarizeWeek(segments, WeekStart, now);

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

        DailySeries =
        [
            new ColumnSeries<double>
            {
                Name = "Active",
                Values = summary.Days.Select(day => day.Active.TotalHours).ToArray(),
                Fill = new SolidColorPaint(new SKColor(0x3B, 0x82, 0xF6)),
                MaxBarWidth = 32
            },
            new ColumnSeries<double>
            {
                Name = "Idle",
                Values = summary.Days.Select(day => day.Idle.TotalHours).ToArray(),
                Fill = new SolidColorPaint(new SKColor(0x9C, 0xA3, 0xAF)),
                MaxBarWidth = 32
            }
        ];

        XAxes =
        [
            new Axis
            {
                Labels = summary.Days.Select(day => day.Date.ToString("ddd")).ToArray(),
                ForceStepToMin = true,
                MinStep = 1
            }
        ];
    }
}
