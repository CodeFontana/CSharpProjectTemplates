using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleUI;

public class App : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<App> _logger;

    public App(IHostApplicationLifetime hostApplicationLifetime,
               ILogger<App> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                await Task.Yield(); // https://github.com/dotnet/runtime/issues/36063
                await Task.Delay(1000, cancellationToken); // Additional delay for Microsoft.Hosting.Lifetime messages
                Execute();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception!");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Execute()
    {
        _logger.LogTrace("Hello, Trace!");
        _logger.LogDebug("Hello, Debug!");
        _logger.LogInformation("Hello, World!");
        _logger.LogWarning("Hello, Warning!");
        _logger.LogError("Hello, Error!");
        _logger.LogCritical("Hello, Critical!");
    }
}
