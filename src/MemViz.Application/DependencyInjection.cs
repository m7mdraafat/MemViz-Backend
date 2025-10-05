namespace MemViz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add application services here, e.g.:
        // services.AddTransient<IMyService, MyService>();

        return services;
    }
}