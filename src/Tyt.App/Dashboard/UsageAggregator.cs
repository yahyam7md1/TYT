using Tyt.Core.Models;

namespace Tyt.App.Dashboard;

public sealed class UsageAggregator
{
    public DaySummary SummarizeDay(IReadOnlyList<Segment> segments, DateOnly date, DateTime now)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var active = TimeSpan.Zero;
        var idle = TimeSpan.Zero;
        var byApp = new Dictionary<string, (AppInfo App, TimeSpan Time)>(StringComparer.Ordinal);

        foreach (var segment in segments)
        {
            var duration = Overlap(segment, dayStart, dayEnd, now);
            if (duration <= TimeSpan.Zero)
            {
                continue;
            }

            if (segment.Kind == SegmentKind.Idle)
            {
                idle += duration;
                continue;
            }

            active += duration;
            if (segment.App is null)
            {
                continue;
            }

            if (!byApp.TryGetValue(segment.App.Id, out var current))
            {
                current = (segment.App, TimeSpan.Zero);
            }

            byApp[segment.App.Id] = (current.App, current.Time + duration);
        }

        var topApps = byApp.Values
            .OrderByDescending(entry => entry.Time)
            .Select(entry => new AppUsage(entry.App, entry.Time))
            .ToArray();

        return new DaySummary(date, active, idle, topApps, active > TimeSpan.Zero || idle > TimeSpan.Zero);
    }

    public WeekSummary SummarizeWeek(IReadOnlyList<Segment> segments, DateOnly weekStart, DateTime now)
    {
        var days = new DaySummary[7];
        var active = TimeSpan.Zero;
        var idle = TimeSpan.Zero;
        var byApp = new Dictionary<string, (AppInfo App, TimeSpan Time)>(StringComparer.Ordinal);

        for (var i = 0; i < 7; i++)
        {
            var date = weekStart.AddDays(i);
            var day = SummarizeDay(segments, date, now);
            days[i] = day;
            active += day.Active;
            idle += day.Idle;

            foreach (var usage in day.TopApps)
            {
                if (!byApp.TryGetValue(usage.App.Id, out var current))
                {
                    current = (usage.App, TimeSpan.Zero);
                }

                byApp[usage.App.Id] = (current.App, current.Time + usage.Active);
            }
        }

        var topApps = byApp.Values
            .OrderByDescending(entry => entry.Time)
            .Select(entry => new AppUsage(entry.App, entry.Time))
            .ToArray();

        return new WeekSummary(
            weekStart,
            WeekCalendar.EndOfWeek(weekStart),
            active,
            idle,
            days,
            topApps,
            active > TimeSpan.Zero || idle > TimeSpan.Zero);
    }

    public static TimeSpan Overlap(Segment segment, DateTime dayStart, DateTime dayEnd, DateTime now)
    {
        var from = segment.StartedAt;
        var to = segment.EndedAt ?? now;
        var start = from > dayStart ? from : dayStart;
        var end = to < dayEnd ? to : dayEnd;
        return end > start ? end - start : TimeSpan.Zero;
    }
}
