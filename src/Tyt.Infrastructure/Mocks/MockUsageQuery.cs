using Tyt.Core.Abstractions;
using Tyt.Core.Models;

namespace Tyt.Infrastructure.Mocks;

public sealed class MockUsageQuery(MockTytState state) : IUsageQuery
{
    public event EventHandler? Changed
    {
        add => state.Changed += value;
        remove => state.Changed -= value;
    }

    public IReadOnlyList<Segment> GetDay(DateOnly date) => GetRange(date, date);

    public IReadOnlyList<Segment> GetRange(DateOnly from, DateOnly toInclusive)
    {
        var rangeStart = from.ToDateTime(TimeOnly.MinValue);
        var rangeEnd = toInclusive.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var now = DateTime.Now;

        return state.Segments
            .Where(segment => Overlaps(segment, rangeStart, rangeEnd, now))
            .OrderBy(segment => segment.StartedAt)
            .ToArray();
    }

    private static bool Overlaps(Segment segment, DateTime rangeStart, DateTime rangeEnd, DateTime now)
    {
        var from = segment.StartedAt;
        var to = segment.EndedAt ?? now;
        return from < rangeEnd && to > rangeStart;
    }
}
