using FluentValidation;
using MemViz.Application.DTOs;

namespace MemViz.Application.Validators;

/// <summary>
/// Validator for SimulationRequestDto
/// </summary>
public class SimulationRequestValidator : AbstractValidator<SimulationRequestDto>
{
    public SimulationRequestValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage("Language is required")
            .MaximumLength(50)
            .WithMessage("Language name must not exceed 50 characters")
            .MustAsync(BeASupportedLanguageAsync)
            .WithMessage("Language '{PropertyValue}' is not supported. Supported languages: {SupportedLanguages}")
            .WithName("Language");

        RuleFor(x => x.SourceCode)
            .NotEmpty()
            .WithMessage("Source code is required")
            .MaximumLength(10000)
            .WithMessage("Source code must not exceed 10,000 characters")
            .MustAsync(BeValidSourceCodeAsync)
            .WithMessage("Source code contains syntax errors: {ValidationError}")
            .WithName("SourceCode");
    }

    private async Task<bool> BeASupportedLanguageAsync(string language, CancellationToken cancellationToken)
    {
        try
        {
            // _languageRegistry.GetAdapter(language);
            return true;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }

    private async Task<bool> BeValidSourceCodeAsync(
        SimulationRequestDto dto,
        string sourceCode,
        ValidationContext<SimulationRequestDto> context,
        CancellationToken cancellationToken)
    {
        try
        {
            // var languageAdapter = _languageRegistry.GetAdapter(dto.Language);
            // var validationResult = await languageAdapter.ValidateSourceCodeAsync(sourceCode, cancellationToken);

            // if (!validationResult.IsSuccess)
            // {
            //     context.MessageFormatter.AppendArgument("ValidationError", string.Join("; ", validationResult.Errors));
            //     return false;
            // }

            return true;
        }
        catch (NotSupportedException)
        {
            // Language validation will catch this
            return true;
        }
    }
}