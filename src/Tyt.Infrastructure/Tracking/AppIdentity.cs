using System.Diagnostics;
using Tyt.Core.Models;

namespace Tyt.Infrastructure.Tracking;

public static class AppIdentity
{
    public static AppInfo? FromSnapshot(ForegroundAppSnapshot? snapshot)
    {
        if (snapshot is null)
        {
            return null;
        }

        var key = snapshot.ExePath ?? snapshot.ProcessName;
        var name = snapshot.ProcessName;
        if (snapshot.ExePath is not null)
        {
            try
            {
                var description = FileVersionInfo.GetVersionInfo(snapshot.ExePath).FileDescription;
                if (!string.IsNullOrWhiteSpace(description))
                {
                    name = description;
                }
            }
            catch (Exception)
            {
                // keep process name
            }
        }

        return new AppInfo
        {
            Id = key.Trim().ToLowerInvariant(),
            Name = string.IsNullOrWhiteSpace(name) ? snapshot.ProcessName : name,
            ExePath = snapshot.ExePath
        };
    }
}
