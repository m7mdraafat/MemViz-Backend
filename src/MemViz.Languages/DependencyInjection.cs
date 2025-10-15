using Microsoft.Extensions.DependencyInjection;

namespace MemViz.Languages;

public static class DependencyInjection
{
    public static IServiceCollection AddLanguageServices(this IServiceCollection services)
    {
        // Language registry and adapters will be registered here when C++ is implemented
        // Core interfaces are available in:
        // - MemViz.Languages.Interfaces (ILanguageAdapter, ILanguageRegistry)
        // - MemViz.Languages.Core (ILanguageParser, ILanguageSimulator)
        // - MemViz.Languages.LanguageModels (SyntaxTree, AST nodes)
        
        return services;
    }
}