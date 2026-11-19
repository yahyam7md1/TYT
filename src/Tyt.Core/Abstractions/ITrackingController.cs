namespace Tyt.Core.Abstractions;

public interface ITrackingController
{
    bool IsPaused { get; }

    void Pause();

    void Resume();

    event EventHandler? StateChanged;
}
