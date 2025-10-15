using MemViz.Application.Interfaces;
using MemViz.Infrastructure.Repositories;
using MemViz.Application.Interfaces;
using MemViz.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MemViz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Repositories - Phase 6 temporary implementations
        services.AddScoped<ISimulationRepository, InMemorySimulationRepository>();
        // Add infrastructure services here, e.g.:
        // services.AddTransient<IMyRepository, MyRepository>();

        return services;
    }
}