using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MinimalApi.Endpoints;
using MinimalApi.Services;
using WebApi.IdentityLibrary.Data;
using WebApi.IdentityLibrary.Entities;
using WebApi.IdentityLibrary.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentityCore<AppUser>(opt =>
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
builder.Services.AddAuthentication("Bearer")
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
            ClockSkew = TimeSpan.FromMinutes(2),
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
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
//builder.Services.AddScoped<UserActivity>(); //////////////////////////////////////////////////////////////////////////////
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
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<JwtBearerSecuritySchemeTransformer>();
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
        context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()?
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
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options =>
    {
        // options.SwaggerEndpoint("/openapi/v2.json", "WebApi v2");
        options.SwaggerEndpoint("/openapi/v1.json", "WebApi v1");
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
app.MapHealthChecks("/health").AllowAnonymous();
app.AddAccountsApiEndpoints();
app.Run();

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

internal sealed class JwtBearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            // Add the security scheme at the document level
            Dictionary<string, IOpenApiSecurityScheme> securitySchemes = new()
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Enter your token (without the 'Bearer ' prefix).",
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            // Apply it as a requirement for all operations
            foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in document.Paths.Values.SelectMany(path => path.Operations!))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}