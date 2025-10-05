using Microsoft.Extensions.DependencyInjection;

namespace MemViz.Languages;

public static class DependencyInjection
{
    public static IServiceCollection AddLanguageServices(this IServiceCollection services)
    {
        // Language registry and adapters will be registered here when C++ is implemented

        return services;
    }
}