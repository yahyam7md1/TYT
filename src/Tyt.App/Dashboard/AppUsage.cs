using Tyt.Core.Models;

namespace Tyt.App.Dashboard;

public sealed record AppUsage(AppInfo App, TimeSpan Active);
