using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace MemViz.Api.Configurations;

/// <summary>
/// Logging configuration for structured logging with Serilog
/// </summary>
public static class LoggingConfiguration
{
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "MemViz.Api")
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(new JsonFormatter())
            .WriteTo.File(
                path: "logs/memviz-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                formatter: new JsonFormatter())
            .WriteTo.Conditional(
                condition: logEvent => logEvent.Level >= LogEventLevel.Warning,
                configureSink: sink => sink.File(
                    path: "logs/errors/memviz-errors-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    formatter: new JsonFormatter()))
            .CreateLogger();

        // Add Serilog to services
        services.AddSerilog();

        // Add custom logging services
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddScoped<SimulationLogContext>();

        return services;
    }

    public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
    {
        // Add Serilog request logging
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId", httpContext.User.Identity.Name);
                }
            };
        });

        return app;
    }
}

/// <summary>
/// Scoped service for maintaining simulation-specific logging context
/// </summary>
public class SimulationLogContext
{
    private readonly ILogger<SimulationLogContext> _logger;
    private string? _currentSimulationId;

    public SimulationLogContext(ILogger<SimulationLogContext> logger)
    {
        _logger = logger;
    }

    public IDisposable BeginScope(Guid simulationId)
    {
        _currentSimulationId = simulationId.ToString();
        return _logger.BeginScope(new Dictionary<string, object>
        {
            { "SimulationId", simulationId },
            { "OperationTimestamp", DateTimeOffset.UtcNow }
        });
    }

    public void LogSimulationEvent(string eventName, object? data = null)
    {
        _logger.LogInformation("Simulation Event: {EventName} for {SimulationId}. Data: {@Data}",
            eventName, _currentSimulationId, data);
    }

    public void LogSimulationError(Exception exception, string operation)
    {
        _logger.LogError(exception, "Simulation Error during {Operation} for {SimulationId}",
            operation, _currentSimulationId);
    }

    public void LogPerformanceMetric(string metricName, double value, string unit = "ms")
    {
        _logger.LogInformation("Performance Metric: {MetricName} = {Value} {Unit} for {SimulationId}",
            metricName, value, unit, _currentSimulationId);
    }
}