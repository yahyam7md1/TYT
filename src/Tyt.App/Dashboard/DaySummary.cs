namespace Tyt.App.Dashboard;

public sealed record DaySummary(
    DateOnly Date,
    TimeSpan Active,
    TimeSpan Idle,
    IReadOnlyList<AppUsage> TopApps,
    bool HasData);
