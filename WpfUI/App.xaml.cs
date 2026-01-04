using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using WpfUI.ViewModels;

namespace WpfUI;

public partial class App : Application
{
    private IHost? _appHost;

    public App()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Default", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            _appHost = Host.CreateDefaultBuilder()
                .UseSerilog((context, services, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
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

            Log.Fatal(ex, "Host terminated unexpectedly");
            Application.Current.Shutdown();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (_appHost == null)
        {
            Log.Fatal("Host was not created. Aborting startup.");
            Application.Current.Shutdown();
            base.OnStartup(e);
            return;
        }

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
