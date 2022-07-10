using DataLibrary.Data;
using DataLibrary.Entities;
using DataLibrary.Identity;
using FileLoggerLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Filters;
using WebApi.Interfaces;
using WebApi.Middleware;
using WebApi.Services;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddFileLogger(builder.Configuration);
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
                    ClockSkew = TimeSpan.FromMinutes(10)
                };
            });

        builder.Services.AddAuthorization(config =>
        {
            config.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddScoped<SeedData>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IAccountService, AccountService>();
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

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FiApi", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Specify JWT bearer token",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        WebApplication app = builder.Build();
        await ApplyDbMigrations(app);

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
        }

        app.UseHttpsRedirection();
        app.UseCors("OpenCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
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
