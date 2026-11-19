using Tyt.Core.Abstractions;
using Tyt.Core.Models;

namespace Tyt.Infrastructure.Mocks;

public sealed class InMemorySettingsStore(MockTytState state) : ISettingsStore
{
    public event EventHandler? Changed
    {
        add => state.Changed += value;
        remove => state.Changed -= value;
    }

    public TrackingSettings Get() => new()
    {
        IdleThreshold = state.Settings.IdleThreshold,
        LaunchAtStartup = state.Settings.LaunchAtStartup
    };

    public void Save(TrackingSettings settings)
    {
        state.Settings = new TrackingSettings
        {
            IdleThreshold = settings.IdleThreshold,
            LaunchAtStartup = settings.LaunchAtStartup
        };
        state.NotifyChanged();
    }
}
