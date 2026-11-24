using System.Diagnostics;
using Tyt.Infrastructure.Tracking.Native;

namespace Tyt.Infrastructure.Tracking;

/// <summary>
/// Reads the window the user is looking at right now.
/// </summary>
public sealed class ForegroundAppProbe
{
    public ForegroundAppSnapshot? Read()
    {
        var hwnd = User32.GetForegroundWindow();
        if (hwnd == IntPtr.Zero)
        {
            return null;
        }

        User32.GetWindowThreadProcessId(hwnd, out var processId);
        if (processId == 0)
        {
            return null;
        }

        try
        {
            using var process = Process.GetProcessById((int)processId);
            string? exe = null;
            try
            {
                exe = process.MainModule?.FileName;
            }
            catch (Exception)
            {
                // Some system/elevated processes refuse MainModule without admin.
            }

            return new ForegroundAppSnapshot
            {
                ProcessId = process.Id,
                ProcessName = process.ProcessName,
                ExePath = exe,
                WindowTitle = string.IsNullOrWhiteSpace(process.MainWindowTitle)
                    ? null
                    : process.MainWindowTitle
            };
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
