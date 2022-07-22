namespace WpfUI;

public partial class App : Application
{
    private IHost _appHost;

    public App()
    {
        try
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";

            _appHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", true, true);
                    config.AddJsonFile($"appSettings.{env}.json", true, true);
                    config.AddUserSecrets<App>(optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddFileLogger(context.Configuration);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<MainViewModel>();
                    services.AddSingleton(sp => new MainWindow(sp.GetRequiredService<MainViewModel>()));
                })
                .Build();
        }
        catch (Exception ex)
        {
            string type = ex.GetType().Name;
            if (type.Equals("StopTheHostException", StringComparison.Ordinal))
            {
                throw;
            }

            Console.WriteLine($"Unexpected error: {ex.Message}");
            Application.Current.Shutdown();
        }
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _appHost.StartAsync();
        using IServiceScope scope = _appHost.Services.CreateScope();
        MainWindow mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        mainWindow.Show();
        mainWindow.ToggleMenu.IsChecked = true;
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_appHost != null)
            {
                await _appHost.StopAsync();
                _appHost.Dispose();
            } 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
        finally
        {
            base.OnExit(e);
        }
    }
}
