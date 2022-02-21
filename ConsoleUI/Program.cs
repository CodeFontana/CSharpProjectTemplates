using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleUI;

class Program
{
    static async Task Main(string[] args)
    {
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
