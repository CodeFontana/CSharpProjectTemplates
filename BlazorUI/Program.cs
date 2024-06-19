using BlazorUI.Components;
using BlazorUI.Interfaces;
using BlazorUI.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddKeyedTransient<IDemoService, DemoService>("Transient");
builder.Services.AddKeyedScoped<IDemoService, DemoService>("Scoped");
builder.Services.AddKeyedSingleton<IDemoService, DemoService>("Singleton");
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment() == false)
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
