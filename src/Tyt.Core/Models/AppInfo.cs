namespace Tyt.Core.Models;

public sealed class AppInfo
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? ExePath { get; init; }
}
