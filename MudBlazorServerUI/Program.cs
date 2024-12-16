using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Services;
using MudBlazorServerUI.Features;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Default", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddHubOptions(options =>
        {
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        });
    builder.Services.AddResponseCompression();
    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = false;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 10000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    });
    builder.Services.AddBlazoredLocalStorage();
    WebApplication app = builder.Build();
    
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    if (app.Environment.IsDevelopment() == false)
    {
        app.UseHsts();
        app.UseResponseCompression();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAntiforgery();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
