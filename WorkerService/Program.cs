using Serilog;
using WorkerService;

string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u4} [{SourceContext}] {Message:lj}{NewLine}{Exception}";

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
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        })
        .UseSerilog((context, services, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration).WriteTo.File(
                path: $@"{Path.GetDirectoryName(Environment.ProcessPath)}\WorkerService-.log",
                outputTemplate: outputTemplate,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1073741824,
                retainedFileCountLimit: 31,
                rollingInterval: RollingInterval.Day))
        .UseWindowsService()
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected error");
}
finally
{
    Log.CloseAndFlush();
}
