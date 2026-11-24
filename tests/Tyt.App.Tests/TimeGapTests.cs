using Tyt.Infrastructure.Tracking;

namespace Tyt.App.Tests;

public class TimeGapTests
{
    [Fact]
    public void FirstSample_IsNotAGap()
    {
        var now = new DateTime(2026, 11, 24, 16, 0, 0);
        Assert.False(TimeGap.Detected(default, now));
    }

    [Fact]
    public void OneSecondTick_IsNotAGap()
    {
        var last = new DateTime(2026, 11, 24, 16, 0, 0);
        Assert.False(TimeGap.Detected(last, last.AddSeconds(1)));
    }

    [Fact]
    public void SleepJump_IsAGap()
    {
        var last = new DateTime(2026, 11, 24, 14, 0, 0);
        var woke = last.AddHours(2);
        Assert.True(TimeGap.Detected(last, woke));
    }
}
