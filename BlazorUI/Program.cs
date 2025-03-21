using BlazorUI.Features;
using BlazorUI.Services;

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
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddKeyedTransient<IDemoService, DemoService>("Transient");
builder.Services.AddKeyedScoped<IDemoService, DemoService>("Scoped");
builder.Services.AddKeyedSingleton<IDemoService, DemoService>("Singleton");
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
