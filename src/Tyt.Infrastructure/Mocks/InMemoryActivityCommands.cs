using Tyt.Core.Abstractions;

namespace Tyt.Infrastructure.Mocks;

public sealed class InMemoryActivityCommands(MockTytState state) : IActivityCommands
{
    public void DeleteAll()
    {
        state.Segments.Clear();
        state.NotifyChanged();
    }
}
