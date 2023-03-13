using ConsoleUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Default", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder();
    
    builder.Services.AddLogging(config =>
    {
        config.ClearProviders();

        Logger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        config.AddSerilog(logger);
    });

    builder.Services.AddHostedService<App>();
    IHost app = builder.Build();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
