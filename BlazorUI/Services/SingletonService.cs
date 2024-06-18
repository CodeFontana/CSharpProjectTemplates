using BlazorUI.Interfaces;

namespace BlazorUI.Services;

internal sealed class SingletonService : ISingletonService
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
