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

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

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

				_appHost = Host.CreateDefaultBuilder(e.Args)
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
						//services.AddSingleton<MainWindowViewModel>();
						services.AddSingleton<MainWindow>();
					})
					.UseSerilog((context, services, loggerConfiguration) =>
						loggerConfiguration.ReadFrom.Configuration(context.Configuration))
					.Build();

				await _appHost.StartAsync();

				var appWindow = _appHost.Services.GetService<MainWindow>();
				//appWindow.DataContext = _appHost.Services.GetService<MainWindowViewModel>();
				appWindow.Show();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Unexpected error");
			}
		}

		protected override async void OnExit(ExitEventArgs e)
		{
            try
            {
				await _appHost.StopAsync();
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
