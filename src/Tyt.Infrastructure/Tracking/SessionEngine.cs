using Tyt.Core.Models;

namespace Tyt.Infrastructure.Tracking;

/// <summary>
/// Open/close rules only. No Windows APIs and no SQLite.
/// Apply() returns the segment that just closed, if any.
/// </summary>
public sealed class SessionEngine
{
    public Segment? Current { get; private set; }

    public void Reset() => Current = null;

    public Segment? Apply(DateTime now, bool isIdle, AppInfo? foregroundApp)
    {
        var kind = isIdle ? SegmentKind.Idle : SegmentKind.Active;
        var app = isIdle ? null : foregroundApp;

        if (Current is null)
        {
            Current = Open(now, kind, app);
            return null;
        }

        if (Current.Kind == kind && SameApp(Current.App, app))
        {
            return null;
        }

        var closed = CloseCurrent(now);
        Current = Open(now, kind, app);
        return closed;
    }

    public Segment? CloseCurrent(DateTime now)
    {
        if (Current is null)
        {
            return null;
        }

        var ended = now < Current.StartedAt ? Current.StartedAt : now;
        var closed = new Segment
        {
            App = Current.App,
            Kind = Current.Kind,
            StartedAt = Current.StartedAt,
            EndedAt = ended
        };
        Current = null;
        return closed;
    }

    private static Segment Open(DateTime now, SegmentKind kind, AppInfo? app) =>
        new()
        {
            App = app,
            Kind = kind,
            StartedAt = now,
            EndedAt = null
        };

    private static bool SameApp(AppInfo? left, AppInfo? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return string.Equals(left.Id, right.Id, StringComparison.OrdinalIgnoreCase);
    }
}
