using BlazorUI.Interfaces;

namespace BlazorUI.Services;

internal sealed class ScopedService : IScopedService
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
