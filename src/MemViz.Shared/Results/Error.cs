namespace MemViz.Shared.Results;

public sealed record Error
{
    public string Code { get; init; }
    public string Message { get; init; }
    public ErrorType Type { get; init; }
    public Dictionary<string, object>? MetaData { get; init; }

    private Error(string code, string message, ErrorType type, Dictionary<string, object>? metaData = null)
    {
        Code = code;
        Message = message;
        Type = type;
        MetaData = metaData;
    }

    public static Error None => new(string.Empty, string.Empty, ErrorType.None);
    public static Error Failure(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Failure, metaData);

    public static Error Validation(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Validation, metaData);

    public static Error NotFound(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.NotFound, metaData);

    public static Error Conflict(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Conflict, metaData);

    public static Error Unauthorized(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Unauthorized, metaData);

    public static Error Forbidden(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Forbidden, metaData);

    public static Error Persistence(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Persistence, metaData);
        
    public static Error Unexpected(string code, string message, Dictionary<string, object>? metaData = null)
        => new(code, message, ErrorType.Unexpected, metaData);
}