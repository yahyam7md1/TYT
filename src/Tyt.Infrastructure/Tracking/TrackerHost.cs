using Tyt.Core.Models;
using Tyt.Infrastructure.Persistence;

namespace Tyt.Infrastructure.Tracking;

public sealed class TrackerHost : IDisposable
{
    private readonly SessionEngine _engine = new();
    private readonly ForegroundAppProbe _foreground = new();
    private readonly IdleProbe _idle = new();
    private readonly SegmentStore _store;
    private readonly SqliteSettingsStore _settings;
    private readonly SqliteUsageQuery _query;
    private readonly SynchronizationContext? _ui;
    private readonly object _gate = new();
    private CancellationTokenSource? _cts;
    private Task? _loop;
    private DateTime _lastCheckpoint = DateTime.MinValue;
    private DateTime _lastSample;
    private bool _paused = true;
    private bool _hasOpenRow;

    public TrackerHost(SegmentStore store, SqliteSettingsStore settings, SqliteUsageQuery query)
    {
        _store = store;
        _settings = settings;
        _query = query;
        _ui = SynchronizationContext.Current;
    }

    public void Start(bool paused)
    {
        _paused = paused;
        _cts = new CancellationTokenSource();
        _loop = Task.Run(() => LoopAsync(_cts.Token));
    }

    public void Pause()
    {
        lock (_gate)
        {
            _paused = true;
            CloseLocked(DateTime.Now);
        }

        NotifyUi();
    }

    public void Resume()
    {
        lock (_gate)
        {
            _paused = false;
        }
    }

    public void Reset()
    {
        lock (_gate)
        {
            _engine.Reset();
            _store.ForgetOpen();
            _hasOpenRow = false;
        }
    }

    public void HandleSuspend()
    {
        lock (_gate)
        {
            CloseLocked(CloseTimestamp(DateTime.Now));
        }

        NotifyUi();
    }

    public void Dispose()
    {
        try
        {
            _cts?.Cancel();
            _loop?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (Exception)
        {
            // shutting down
        }

        lock (_gate)
        {
            CloseLocked(DateTime.Now);
        }

        _cts?.Dispose();
    }

    private async Task LoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var changed = Tick();
                if (changed)
                {
                    NotifyUi();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private bool Tick()
    {
        lock (_gate)
        {
            if (_paused)
            {
                _lastSample = DateTime.Now;
                return false;
            }

            var now = DateTime.Now;
            var changed = false;

            if (TimeGap.Detected(_lastSample, now))
            {
                CloseLocked(_lastSample);
                changed = true;
            }

            var isIdle = _idle.ReadIdleTime() >= _settings.Get().IdleThreshold;
            var snapshot = isIdle ? null : _foreground.Read();
            var app = AppIdentity.FromSnapshot(snapshot);
            var closed = _engine.Apply(now, isIdle, app);

            if (closed is not null)
            {
                _store.CloseOpen(closed);
                _hasOpenRow = false;
                changed = true;
            }

            if (_engine.Current is not null && !_hasOpenRow)
            {
                _store.InsertOpen(_engine.Current);
                _hasOpenRow = true;
                _lastCheckpoint = now;
                changed = true;
            }
            else if (_engine.Current is not null && now - _lastCheckpoint >= TimeSpan.FromSeconds(30))
            {
                _store.CheckpointOpen(now);
                _lastCheckpoint = now;
            }

            _lastSample = now;
            return changed;
        }
    }

    private DateTime CloseTimestamp(DateTime fallback) =>
        _lastSample == default ? fallback : _lastSample;

    private void CloseLocked(DateTime now)
    {
        var closed = _engine.CloseCurrent(now);
        if (closed is not null)
        {
            _store.CloseOpen(closed);
        }

        _hasOpenRow = false;
    }

    private void NotifyUi()
    {
        if (_ui is null)
        {
            _query.NotifyChanged();
            return;
        }

        _ui.Post(_ => _query.NotifyChanged(), null);
    }
}
