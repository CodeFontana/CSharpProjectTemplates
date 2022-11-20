using DataLibrary.Entities;
using DataLibrary.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;
using AspNetCoreRateLimit;
using DataLibrary.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using WebApi.Filters;
using WebApi.Interfaces;
using WebApi.Middleware;
using WebApi.Services;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
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
                                builder.Configuration.GetValue<string>("Authentication:JwtSecurityKey"))),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(10)
                    };
                });

            builder.Services.AddAuthorization(config =>
            {
                config.AddPolicy("Administrator", policy =>
                {
                    policy.RequireRole("Administrator");
                });

                config.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

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

            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new(1, 0);
                options.ReportApiVersions = true;
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddHealthChecks()
                            .AddDbContextCheck<IdentityContext>("Identity Database Health Check")
                            .AddSqlServer(builder.Configuration.GetConnectionString("Default"));

            // Pending .NET 7 support
            //builder.Services.AddHealthChecksUI(options =>
            //{
            //    options.AddHealthCheckEndpoint("WebAPI", "/health");
            //    options.SetEvaluationTimeInSeconds(60);
            //    options.SetMinimumSecondsBetweenFailureNotifications(600);
            //}).AddInMemoryStorage();

            builder.Services.Configure<IpRateLimitOptions>(
                builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            builder.Services.AddInMemoryRateLimiting();

            WebApplication app = builder.Build();
            await ApplyDbMigrations(app);

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // options.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApi v2");
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("OpenCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.MapControllers();
            app.UseIpRateLimiting();
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }).AllowAnonymous();
            // Pending .NET 7 support
            //app.MapHealthChecksUI(setup =>
            //{
            //    setup.AddCustomStylesheet("Resources\\health-ui.css");
            //}).AllowAnonymous();

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
    }

    public static async Task ApplyDbMigrations(WebApplication app)
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
}
