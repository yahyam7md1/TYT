namespace Tyt.Core.Models;

public sealed class TrackingSettings
{
    public TimeSpan IdleThreshold { get; set; } = TimeSpan.FromMinutes(2);

    public bool LaunchAtStartup { get; set; }
}
