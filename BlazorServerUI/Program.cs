WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

builder.Logging.ClearProviders();
builder.Logging.AddConsoleLogger(builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(); 
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment() == false)
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
