using Tyt.Core.Models;
using Tyt.Infrastructure.Tracking;

namespace Tyt.App.Tests;

public class SessionEngineTests
{
    private static readonly AppInfo Vs = new() { Id = "vs", Name = "Visual Studio" };
    private static readonly AppInfo Chrome = new() { Id = "chrome", Name = "Chrome" };

    [Fact]
    public void SwitchApp_ClosesPrevious_WithoutOverlap()
    {
        var engine = new SessionEngine();
        var t0 = new DateTime(2026, 11, 20, 9, 0, 0);
        Assert.Null(engine.Apply(t0, isIdle: false, Vs));
        var closed = engine.Apply(t0.AddMinutes(30), isIdle: false, Chrome);

        Assert.NotNull(closed);
        Assert.Equal(Vs.Id, closed!.App!.Id);
        Assert.Equal(t0, closed.StartedAt);
        Assert.Equal(t0.AddMinutes(30), closed.EndedAt);
        Assert.Equal(Chrome.Id, engine.Current!.App!.Id);
        Assert.Null(engine.Current.EndedAt);
    }

    [Fact]
    public void Idle_ClosesActive_AndDoesNotCountApp()
    {
        var engine = new SessionEngine();
        var t0 = new DateTime(2026, 11, 20, 12, 0, 0);
        engine.Apply(t0, isIdle: false, Vs);
        var closed = engine.Apply(t0.AddMinutes(2), isIdle: true, Vs);

        Assert.Equal(SegmentKind.Active, closed!.Kind);
        Assert.Equal(SegmentKind.Idle, engine.Current!.Kind);
        Assert.Null(engine.Current.App);
    }

    [Fact]
    public void SameApp_DoesNotClose()
    {
        var engine = new SessionEngine();
        var t0 = new DateTime(2026, 11, 20, 9, 0, 0);
        engine.Apply(t0, isIdle: false, Vs);
        Assert.Null(engine.Apply(t0.AddSeconds(1), isIdle: false, Vs));
        Assert.Equal(t0, engine.Current!.StartedAt);
    }
}
