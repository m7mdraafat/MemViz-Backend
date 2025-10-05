using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace MemViz.Languages;

public static class DependencyInjection
{
    public static IServiceCollection AddLanguagesServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Language registry and adapters will be registered here when C++ is implemented

        return services;
    }
}