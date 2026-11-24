using Tyt.Infrastructure.Tracking.Native;

namespace Tyt.Infrastructure.Tracking;

/// <summary>
/// How long since the last mouse/keyboard input, according to Windows.
/// </summary>
public sealed class IdleProbe
{
    public TimeSpan ReadIdleTime()
    {
        var info = new LastInputInfo
        {
            Size = (uint)System.Runtime.InteropServices.Marshal.SizeOf<LastInputInfo>()
        };

        if (!User32.GetLastInputInfo(ref info))
        {
            return TimeSpan.Zero;
        }

        var nowTick = unchecked((uint)Environment.TickCount);
        var idleMs = nowTick - info.LastInputTick;
        return TimeSpan.FromMilliseconds(idleMs);
    }
}
