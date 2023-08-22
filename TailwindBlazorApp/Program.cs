using Blazored.LocalStorage;
using TailwindBlazorApp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddServerComponents();
builder.Services.AddBlazoredLocalStorage();
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment() == false)
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorComponents<App>();
app.Run();
