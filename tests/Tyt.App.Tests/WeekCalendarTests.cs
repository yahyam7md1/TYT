using Tyt.App.Dashboard;

namespace Tyt.App.Tests;

public class WeekCalendarTests
{
    [Theory]
    [InlineData(2026, 11, 16, 2026, 11, 16)]
    [InlineData(2026, 11, 20, 2026, 11, 16)]
    [InlineData(2026, 11, 22, 2026, 11, 16)]
    public void StartOfWeek_IsMonday(int y, int m, int d, int ey, int em, int ed)
    {
        var start = WeekCalendar.StartOfWeek(new DateOnly(y, m, d));
        Assert.Equal(new DateOnly(ey, em, ed), start);
        Assert.Equal(DayOfWeek.Monday, start.DayOfWeek);
    }
}
