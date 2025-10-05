using MemViz.Api.Configurations;
using MemViz.Api.Middleware;
using MemViz.Application;
using MemViz.Infrastructure;
using MemViz.Languages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add swagger
builder.Services.AddSwaggerConfiguration();

// Add CORS
builder.Services.AddCorsConfiguration(builder.Configuration);

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddLanguagesServices(builder.Configuration);

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MemViz API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at root
    });
}

// Add custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// Use CORS
app.UseCorsConfiguration();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Logger.LogInformation("MemViz API starting...");
app.Run();
