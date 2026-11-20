namespace Tyt.Wpf.ViewModels;

public sealed class TimelineBlock
{
    public required string Title { get; init; }

    public required string KindLabel { get; init; }

    public required string Range { get; init; }

    public required string Duration { get; init; }

    public required bool IsIdle { get; init; }

    public required bool IsOpen { get; init; }

    public required double BarHeight { get; init; }

    public string Detail =>
        $"{Title} ({KindLabel})\n{Range}\n{Duration}";
}
