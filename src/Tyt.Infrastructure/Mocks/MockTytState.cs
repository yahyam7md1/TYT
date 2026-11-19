using Tyt.Core.Models;

namespace Tyt.Infrastructure.Mocks;

public sealed class MockTytState
{
    public List<Segment> Segments { get; } = [];

    public TrackingSettings Settings { get; set; } = new();

    public bool IsPaused { get; set; }

    public event EventHandler? Changed;

    public void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

    public static MockTytState CreateSeeded()
    {
        var state = new MockTytState();
        var today = DateTime.Today;

        var vs = new AppInfo { Id = "devenv", Name = "Visual Studio", ExePath = @"C:\Program Files\Microsoft Visual Studio\devenv.exe" };
        var chrome = new AppInfo { Id = "chrome", Name = "Google Chrome", ExePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe" };
        var slack = new AppInfo { Id = "slack", Name = "Slack", ExePath = @"C:\Users\user\AppData\Local\slack\slack.exe" };
        var cursor = new AppInfo { Id = "cursor", Name = "Cursor", ExePath = @"C:\Users\user\AppData\Local\Programs\cursor\Cursor.exe" };

        state.Segments.AddRange(
        [
            Active(vs, today, 9, 0, 10, 30),
            Active(chrome, today, 10, 30, 11, 15),
            Active(vs, today, 11, 15, 12, 0),
            Idle(today, 12, 0, 12, 45),
            Active(cursor, today, 12, 45, 14, 0),
            Active(slack, today, 14, 0, 14, 20),
            Active(cursor, today, 14, 20, 16, 0),
            new Segment
            {
                App = chrome,
                Kind = SegmentKind.Active,
                StartedAt = today.AddHours(16),
                EndedAt = null
            }
        ]);

        return state;
    }

    private static Segment Active(AppInfo app, DateTime day, int startH, int startM, int endH, int endM) =>
        new()
        {
            App = app,
            Kind = SegmentKind.Active,
            StartedAt = day.AddHours(startH).AddMinutes(startM),
            EndedAt = day.AddHours(endH).AddMinutes(endM)
        };

    private static Segment Idle(DateTime day, int startH, int startM, int endH, int endM) =>
        new()
        {
            Kind = SegmentKind.Idle,
            StartedAt = day.AddHours(startH).AddMinutes(startM),
            EndedAt = day.AddHours(endH).AddMinutes(endM)
        };
}
