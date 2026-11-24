namespace Tyt.Infrastructure.Tracking;

public sealed class ForegroundAppSnapshot
{
    public required int ProcessId { get; init; }
    public required string ProcessName { get; init; }
    public string? ExePath { get; init; }
    public string? WindowTitle { get; init; }
}
