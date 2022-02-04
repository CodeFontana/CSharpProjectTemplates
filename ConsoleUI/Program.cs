using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleUI;

class Program
{
    public static IConfigurationRoot? Configuration { get; set; }

    static async Task Main(string[] args)
    {
        string? env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        bool isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";

        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", true, true);
                config.AddJsonFile($"appsettings.{env}.json", true, true);
                config.AddEnvironmentVariables();

                if (isDevelopment)
                {
                    config.AddUserSecrets<Program>(optional: true);
                }
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<App>();
            })
            .ConfigureLogging((hostContext, config) =>
            {
                config.ClearProviders();
                config.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "MM/dd/yyyy HH:mm:ss ";
                });
            })
            .RunConsoleAsync();
    }
}
