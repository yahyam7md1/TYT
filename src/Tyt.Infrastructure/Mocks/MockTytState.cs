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

        var monday = today.AddDays(-(((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7));

        void AddPastDay(DateTime day, params Segment[] daySegments)
        {
            if (day.Date >= today)
            {
                return;
            }

            state.Segments.AddRange(daySegments);
        }

        AddPastDay(monday,
            Active(vs, monday, 9, 0, 12, 0),
            Active(chrome, monday, 12, 0, 13, 0),
            Idle(monday, 13, 0, 13, 30));

        AddPastDay(monday.AddDays(1),
            Active(cursor, monday.AddDays(1), 10, 0, 14, 0),
            Active(slack, monday.AddDays(1), 14, 0, 15, 0));

        AddPastDay(monday.AddDays(3),
            Active(chrome, monday.AddDays(3), 9, 0, 11, 0),
            Active(vs, monday.AddDays(3), 11, 0, 13, 0),
            Idle(monday.AddDays(3), 13, 0, 14, 0));

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
