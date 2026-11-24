using System.Runtime.InteropServices;

namespace Tyt.Infrastructure.Tracking.Native;

/// <summary>
/// Thin wrappers around Windows user32 functions.
/// This is not the tracker yet — just the two questions TYT needs to ask the OS.
/// </summary>
internal static class User32
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    public static extern bool GetLastInputInfo(ref LastInputInfo info);
}

[StructLayout(LayoutKind.Sequential)]
internal struct LastInputInfo
{
    public uint Size;
    public uint LastInputTick;
}
