using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using WebApi.Filters;
using WebApi.IdentityLibrary.Data;
using WebApi.IdentityLibrary.Entities;
using WebApi.IdentityLibrary.Identity;
using WebApi.Interfaces;
using WebApi.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Default", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    builder.Services.AddDbContext<IdentityContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    });
    builder.Services
        .AddIdentityCore<AppUser>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 6;
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddSignInManager<SignInManager<AppUser>>()
        .AddRoleValidator<RoleValidator<AppRole>>()
        .AddEntityFrameworkStores<IdentityContext>();
    builder.Services
        .AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration.GetValue<string>("Authentication:JwtIssuer"),
                ValidateAudience = true,
                ValidAudience = builder.Configuration.GetValue<string>("Authentication:JwtAudience"),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        builder.Configuration.GetValue<string>("Authentication:JwtSecurityKey")
                        ?? throw new InvalidOperationException("Configuration is missing JwtSecurityKey"))),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(10)
            };
        });
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("Administrator", policy =>
        {
            policy.RequireRole("Administrator");
        })
        .SetFallbackPolicy(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build());
    builder.Services.AddScoped<SeedData>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddResponseCaching();
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<UserActivity>();
    builder.Services.AddControllers().AddJsonOptions(config =>
    {
        config.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
    builder.Services.AddCors(policy =>
    {
        policy.AddPolicy("OpenCorsPolicy", options =>
            options
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WebApi v1",
            Version = "v1",
            Description = "This is a template API"
        });
        //options.SwaggerDoc("v2", new OpenApiInfo
        //{
        //    Title = "WebApi v2",
        //    Version = "v2",
        //    Description = "This is a template API"
        //});
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Specify JWT bearer token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
    });
    builder.Services
        .AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new(1, 0);
            options.ReportApiVersions = true;
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    builder.Services.AddHealthChecks()
                    .AddDbContextCheck<IdentityContext>("Identity Database Health Check");
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("fixed", limiterOptions =>
        {
            limiterOptions.PermitLimit = 4;
            limiterOptions.Window = TimeSpan.FromSeconds(12);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 0;
        });

        options.OnRejected = (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;
            context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
                .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));
            context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken: cancellationToken);
            return new ValueTask();
        };
    });
    WebApplication app = builder.Build();
    await ApplyDbMigrations(app);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // options.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApi v2");
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
            options.EnableTryItOutByDefault();
            options.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
            {
                ["activated"] = false
            };
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("OpenCorsPolicy");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseResponseCaching();
    app.MapControllers();
    app.MapHealthChecks("/health").AllowAnonymous();
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

static string GetUserEndPoint(HttpContext context) =>
        $"User {context.User.Identity?.Name ?? "Anonymous"}, " +
        $"Endpoint: {context.Request.Path}, " +
        $"IP: {context.Connection.RemoteIpAddress}";

static async Task ApplyDbMigrations(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        IdentityContext identityContext = services.GetRequiredService<IdentityContext>();
        SeedData seedData = services.GetRequiredService<SeedData>();

        logger.LogInformation("Apply database migrations...");
        await identityContext.Database.MigrateAsync();

        logger.LogInformation("Seed database...");
        await seedData.SeedUsersAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occured during database migration");
        Console.ReadKey();
    }
}