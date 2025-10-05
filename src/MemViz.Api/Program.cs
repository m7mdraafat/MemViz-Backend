using Asp.Versioning;
using MemViz.Api.Configurations;
using MemViz.Api.Middleware;
using MemViz.Application;
using MemViz.Infrastructure;
using MemViz.Languages;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
try
{
    // Logging Configuration (must be first)
    builder.Services.AddLoggingConfiguration(builder.Configuration);
    builder.Host.UseSerilog();

    // Core Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    // Application Services
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddLanguageServices();

    // CORS Configuration
    builder.Services.AddCorsConfiguration(builder.Configuration);

    // SignalR
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.MaximumReceiveMessageSize = 64 * 1024; // 64KB
        options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    });

    // Swagger Documentation
    builder.Services.AddSwaggerDocumentation();

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

    // API Versioning (for future use)
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("version"),
            new HeaderApiVersionReader("X-Version"));
    }).AddApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });

    Log.Information("MemViz API services configured successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to configure MemViz API services");
    throw;
}

var app = builder.Build();

// Configure the HTTP request pipeline
try
{
    // Logging
    app.UseLoggingConfiguration();

    // Global Exception Handling
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Development-specific middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseHsts();
    }

    // Security Headers
    app.UseHttpsRedirection();
    // app.UseSecurityHeaders();

    // CORS
    app.UseCors("AllowAll");

    // Static Files (for custom Swagger CSS)
    app.UseStaticFiles();

    // Swagger Documentation
    app.UseSwaggerDocumentation(app.Environment);

    // Authentication & Authorization (for future use)
    app.UseAuthentication();
    app.UseAuthorization();

    // Routing
    app.UseRouting();

    // Controllers
    app.MapControllers();

    // Health Checks
    app.MapHealthChecks("/health");

    // API Info Endpoint
    app.MapGet("/", () => new
    {
        Application = "MemViz API",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        Documentation = "/api/docs",
        Health = "/health",
        SignalRHub = "/hubs/simulation",
        Timestamp = DateTimeOffset.UtcNow
    });

    Log.Information("MemViz API pipeline configured successfully");
    Log.Information("MemViz API starting on {Environment} environment", app.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MemViz API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
