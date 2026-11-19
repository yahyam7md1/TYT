using Tyt.Core.Abstractions;

namespace Tyt.Infrastructure.Mocks;

public sealed class InMemoryTrackingController(MockTytState state) : ITrackingController
{
    public bool IsPaused => state.IsPaused;

    public event EventHandler? StateChanged
    {
        add => state.Changed += value;
        remove => state.Changed -= value;
    }

    public void Pause()
    {
        state.IsPaused = true;
        state.NotifyChanged();
    }

    public void Resume()
    {
        state.IsPaused = false;
        state.NotifyChanged();
    }
}
