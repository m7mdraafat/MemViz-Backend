namespace MemViz.Shared.Constants;

public static class ErrorCodes
{
    // General
    public const string GeneralFailure = "GENERAL.FAILURE";
    public const string GeneralValidation = "GENERAL.VALIDATION";
    public const string GeneralNotFound = "GENERAL.NOT_FOUND";
    public const string GeneralConflict = "GENERAL.CONFLICT";
    public const string GeneralUnauthorized = "GENERAL.UNAUTHORIZED";
    public const string GeneralForbidden = "GENERAL.FORBIDDEN";

    // Execution
    public const string ExecutionFailed = "EXECUTION.FAILED";
    public const string ExecutionTimeout = "EXECUTION.TIMEOUT";
    public const string ExecutionCompilationError = "EXECUTION.COMPILATION_ERROR";
    
    // Language
    public const string LanguageNotSupported = "LANGUAGE.NOT_SUPPORTED";
    public const string LanguageParsingError = "LANGUAGE.PARSING_ERROR";
}