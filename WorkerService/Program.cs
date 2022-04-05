using FileLoggerLibrary;
using WorkerService;

try
{
    string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
    bool isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";

    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(config =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", true, true);
            config.AddJsonFile($"appsettings.{env}.json", true, true);
            config.AddUserSecrets<Program>(optional: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.AddFileLogger(context.Configuration);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        })
        .UseWindowsService()
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}

