using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleUI;

class Program
{
    static async Task Main(string[] args)
    {
        string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u4} [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: outputTemplate)
            .WriteTo.File(
                path: "ConsoleUI-.log",
                outputTemplate: outputTemplate,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1073741824,
                retainedFileCountLimit: 31,
                rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();

        try
        {
            string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            bool isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";

            await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", true, true);
                    config.AddJsonFile($"appsettings.{env}.json", true, true);
                    config.AddUserSecrets<Program>(optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<App>();
                })
                .UseSerilog((context, services, loggerConfiguration) => 
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration))
                .RunConsoleAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unexpected error");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
