namespace Tyt.App.Dashboard;

public sealed record WeekSummary(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    TimeSpan Active,
    TimeSpan Idle,
    IReadOnlyList<DaySummary> Days,
    IReadOnlyList<AppUsage> TopApps,
    bool HasData);
