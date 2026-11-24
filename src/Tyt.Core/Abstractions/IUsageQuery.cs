using Tyt.Core.Models;

namespace Tyt.Core.Abstractions;

public interface IUsageQuery
{
    IReadOnlyList<Segment> GetDay(DateOnly date);

    IReadOnlyList<Segment> GetRange(DateOnly from, DateOnly toInclusive);

    event EventHandler? Changed;
}
