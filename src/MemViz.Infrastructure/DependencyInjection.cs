namespace MemViz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add infrastructure services here, e.g.:
        // services.AddTransient<IMyRepository, MyRepository>();

        return services;
    }
}