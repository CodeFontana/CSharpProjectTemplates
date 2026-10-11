namespace FluentUI.Services;

internal sealed class DemoService : IDemoService
{
    public Guid Guid { get; set; } = Guid.NewGuid();
}
