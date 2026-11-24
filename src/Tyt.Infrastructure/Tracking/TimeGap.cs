namespace Tyt.Infrastructure.Tracking;

public static class TimeGap
{
    /// <summary>
    /// Longer than a normal 1s tick. Sleep, hibernate, or a stuck process.
    /// Close the current segment at the last sample, not at DateTime.Now.
    /// </summary>
    public static readonly TimeSpan Threshold = TimeSpan.FromSeconds(15);

    public static bool Detected(DateTime lastSample, DateTime now) =>
        lastSample != default && now - lastSample > Threshold;
}
