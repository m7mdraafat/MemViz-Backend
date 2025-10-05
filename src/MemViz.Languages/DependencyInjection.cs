namespace MemViz.Languages;

public static class DependencyInjection
{
    public static IServiceCollection AddLanguageServices(this IServiceCollection services)
    {
        // Register language processors
        // services.AddScoped<ILanguageProcessor, CLanguageProcessor>();
        // services.AddScoped<ILanguageProcessor, CppLanguageProcessor>();
        
        // Register language factory
        // services.AddScoped<ILanguageProcessorFactory, LanguageProcessorFactory>();

        return services;
    }
}