using Tyt.Core.Models;

namespace Tyt.Core.Abstractions;

public interface ISettingsStore
{
    TrackingSettings Get();

    void Save(TrackingSettings settings);

    event EventHandler? Changed;
}
