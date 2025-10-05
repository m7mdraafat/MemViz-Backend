using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace MemViz.Languages;

public static class DependencyInjection
{
    public static IServiceCollection AddLanguagesServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register language processors
        // services.AddScoped<ILanguageProcessor, CLanguageProcessor>();
        // services.AddScoped<ILanguageProcessor, CppLanguageProcessor>();
        
        // Register language factory
        // services.AddScoped<ILanguageProcessorFactory, LanguageProcessorFactory>();

        return services;
    }
}