using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Windows;

namespace WpfUI
{
    public partial class App : Application
    {
        private IHost _appHost;

        public App()
        {
            try
            {
                string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                bool isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";

                _appHost = Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration(config =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json", true, true);
                        config.AddJsonFile($"appsettings.{env}.json", true, true);
                        config.AddUserSecrets<App>(optional: true);
                        config.AddEnvironmentVariables();
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddTransient<MainWindow>(s => new MainWindow()
                        {
                            
                        });
                    })
                    .UseSerilog((context, services, loggerConfiguration) =>
                        loggerConfiguration.ReadFrom.Configuration(context.Configuration))
                    .Build();
            }
            catch (Exception ex)
            {
                string type = ex.GetType().Name;
                if (type.Equals("StopTheHostException", StringComparison.Ordinal))
                {
                    throw;
                }

                Log.Fatal(ex, "Unexpected error");
                Application.Current.Shutdown();
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _appHost.StartAsync();
            MainWindow mainWindow = _appHost.Services.GetService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_appHost != null) await _appHost.StopAsync();
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unexpected error");
            }
            finally
            {
                base.OnExit(e);
            }
        }
    }
}
