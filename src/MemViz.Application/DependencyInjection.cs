using MemViz.Application.Interfaces;
using MemViz.Application.Services;
using MemViz.Application.Validators;
using MemViz.Languages;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MemViz.Application.DTOs;

namespace MemViz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add language services first
        services.AddLanguageServices();

        // Core application services
        services.AddScoped<ISimulationOrchestrator, SimulationOrchestrator>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<SimulationRequestValidator>();

        // Auto-discovery of validators
        services.AddScoped<IValidator<SimulationRequestDto>, SimulationRequestValidator>();

        return services;
    }
}