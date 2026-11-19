using Tyt.App.Dashboard;
using Tyt.Core.Models;

namespace Tyt.App.Tests;

public class UsageAggregatorTests
{
    [Fact]
    public void SummarizeDay_SplitsActiveAndIdle_AndRanksApps()
    {
        var date = new DateOnly(2026, 11, 19);
        var day = date.ToDateTime(TimeOnly.MinValue);
        var vs = new AppInfo { Id = "vs", Name = "Visual Studio" };
        var chrome = new AppInfo { Id = "chrome", Name = "Chrome" };

        var segments = new[]
        {
            new Segment { App = vs, Kind = SegmentKind.Active, StartedAt = day.AddHours(9), EndedAt = day.AddHours(10) },
            new Segment { Kind = SegmentKind.Idle, StartedAt = day.AddHours(10), EndedAt = day.AddHours(10).AddMinutes(30) },
            new Segment { App = chrome, Kind = SegmentKind.Active, StartedAt = day.AddHours(10).AddMinutes(30), EndedAt = day.AddHours(11) }
        };

        var summary = new UsageAggregator().SummarizeDay(segments, date, day.AddHours(12));

        Assert.True(summary.HasData);
        Assert.Equal(TimeSpan.FromHours(1.5), summary.Active);
        Assert.Equal(TimeSpan.FromMinutes(30), summary.Idle);
        Assert.Equal("vs", summary.TopApps[0].App.Id);
        Assert.Equal(TimeSpan.FromHours(1), summary.TopApps[0].Active);
    }
}
