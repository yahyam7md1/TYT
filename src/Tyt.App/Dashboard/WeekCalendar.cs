namespace Tyt.App.Dashboard;

public static class WeekCalendar
{
    /// <summary>Monday of the week that contains <paramref name="date"/> (Monday–Sunday weeks).</summary>
    public static DateOnly StartOfWeek(DateOnly date)
    {
        var offset = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-offset);
    }

    public static DateOnly EndOfWeek(DateOnly weekStart) => weekStart.AddDays(6);
}
