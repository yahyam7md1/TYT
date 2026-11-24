using Tyt.Core.Abstractions;
using Tyt.Core.Models;

namespace Tyt.Infrastructure.Persistence;

public sealed class SqliteUsageQuery(SegmentStore store) : IUsageQuery
{
    public event EventHandler? Changed;

    public IReadOnlyList<Segment> GetDay(DateOnly date) => GetRange(date, date);

    public IReadOnlyList<Segment> GetRange(DateOnly from, DateOnly toInclusive)
    {
        var rangeStart = from.ToDateTime(TimeOnly.MinValue);
        var rangeEnd = toInclusive.AddDays(1).ToDateTime(TimeOnly.MinValue);
        return store.GetOverlapping(rangeStart, rangeEnd, DateTime.Now);
    }

    public void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);
}
