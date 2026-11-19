namespace Tyt.Core.Models;

public sealed class Segment
{
    public AppInfo? App { get; init; }

    public required SegmentKind Kind { get; init; }

    public required DateTime StartedAt { get; init; }

    /// <summary>Null means the segment is still open (until now).</summary>
    public DateTime? EndedAt { get; init; }
}
