namespace MemViz.Api.Configurations;

public static class CorsConfiguration
{
    private const string AllowSpecificOrigins = "_allowSpecificOrigins";

    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "http://localhost:5173" };

        services.AddCors(options =>
        {
            options.AddPolicy(AllowSpecificOrigins, builder =>
            {
                builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors(AllowSpecificOrigins);
        return app;
    }
}