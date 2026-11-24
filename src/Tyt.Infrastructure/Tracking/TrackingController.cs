using Tyt.Core.Abstractions;
using Tyt.Infrastructure.Persistence;

namespace Tyt.Infrastructure.Tracking;

public sealed class TrackingController : ITrackingController, IDisposable
{
    private readonly TrackerHost _host;
    private readonly SqliteSettingsStore _settings;

    public TrackingController(TrackerHost host, SqliteSettingsStore settings)
    {
        _host = host;
        _settings = settings;
        IsPaused = _settings.IsPaused();
        _host.Start(IsPaused);
    }

    public bool IsPaused { get; private set; }

    public event EventHandler? StateChanged;

    public void Pause()
    {
        if (IsPaused)
        {
            return;
        }

        IsPaused = true;
        _settings.SetPaused(true);
        _host.Pause();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Resume()
    {
        if (!IsPaused)
        {
            return;
        }

        IsPaused = false;
        _settings.SetPaused(false);
        _host.Resume();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose() => _host.Dispose();
}
