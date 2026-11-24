using Tyt.Infrastructure.Tracking;

var foreground = new ForegroundAppProbe();
var idle = new IdleProbe();
string? lastKey = null;

Console.WriteLine("TYT probe — switch windows and sit still. Ctrl+C to stop.");
Console.WriteLine();

while (true)
{
    var app = foreground.Read();
    var idleFor = idle.ReadIdleTime();
    var key = app is null ? "(none)" : $"{app.ProcessName}:{app.ProcessId}";

    if (key != lastKey)
    {
        lastKey = key;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] foreground → {Describe(app)}");
    }

    Console.Write($"\r        idle {idleFor.TotalSeconds:0}s   ");

    await Task.Delay(1000);
}

static string Describe(ForegroundAppSnapshot? app)
{
    if (app is null)
    {
        return "(no foreground window)";
    }

    var exe = app.ExePath is null ? app.ProcessName : Path.GetFileName(app.ExePath);
    var title = string.IsNullOrWhiteSpace(app.WindowTitle) ? "" : $" | {app.WindowTitle}";
    return $"{exe}{title}";
}
