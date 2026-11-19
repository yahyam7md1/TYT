namespace Tyt.Wpf.Formatting;

public static class DurationFormatter
{
    public static string ToReadable(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
        {
            value = TimeSpan.Zero;
        }

        var hours = (int)value.TotalHours;
        var minutes = value.Minutes;
        if (hours > 0)
        {
            return $"{hours}h {minutes:D2}m";
        }

        return $"{minutes}m";
    }
}
