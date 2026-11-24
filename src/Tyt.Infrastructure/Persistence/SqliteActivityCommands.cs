using Tyt.Core.Abstractions;
using Tyt.Infrastructure.Tracking;

namespace Tyt.Infrastructure.Persistence;

public sealed class SqliteActivityCommands(SegmentStore store, TrackerHost host, SqliteUsageQuery query)
    : IActivityCommands
{
    public void DeleteAll()
    {
        host.Reset();
        store.DeleteAll();
        query.NotifyChanged();
    }
}
