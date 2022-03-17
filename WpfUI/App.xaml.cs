﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Windows;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ViewModels;

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
                        services.AddSingleton<NavigationStore>();
                        services.AddSingleton<NavigationService<HelloWorldViewModel>>();
                        services.AddSingleton<NavigationService<CounterViewModel>>();
                        services.AddSingleton<Func<HelloWorldViewModel>>((s) => () => s.GetRequiredService<HelloWorldViewModel>());
                        services.AddSingleton<Func<CounterViewModel>>((s) => () => s.GetRequiredService<CounterViewModel>());
                        services.AddSingleton<MainViewModel>();
                        services.AddTransient<HelloWorldViewModel>();
                        services.AddTransient<CounterViewModel>();
                        services.AddSingleton(s => new MainWindow()
                        {
                            DataContext = s.GetRequiredService<MainViewModel>()
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
            NavigationService<HelloWorldViewModel> navService = _appHost.Services.GetRequiredService<NavigationService<HelloWorldViewModel>>();
            navService.Navigate();
            MainWindow mainWindow = _appHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            mainWindow.ToggleMenu.IsChecked = true;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_appHost != null) _appHost.Dispose();
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
