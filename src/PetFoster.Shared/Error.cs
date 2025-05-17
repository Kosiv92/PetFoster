namespace PetFoster.Core;

public sealed record Error
{
    public const string SEPARATOR = "||";

    public string Code { get; set; }

    public string Message { get; set; }

    public ErrorType Type { get; set; }

    public string? InvalidField { get; } = null;

    private Error(string code,
        string message,
        ErrorType type,
        string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Validation(string code, string message, string? invalidField = null)
    {
        return new Error(code, message, ErrorType.Validation, invalidField);
    }

    public static Error NotFound(string code, string message)
    {
        return new Error(code, message, ErrorType.NotFound);
    }

    public static Error Failure(string code, string message)
    {
        return new Error(code, message, ErrorType.Failure);
    }

    public static Error Conflict(string code, string message)
    {
        return new Error(code, message, ErrorType.Conflict);
    }

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type);
    }

    public static Error Deserialize(string serialized)
    {
        string[] parts = serialized.Split(SEPARATOR);
        return parts.Length < 3
            ? throw new ArgumentException("Invalid serialized format")
            : Enum.TryParse<ErrorType>(parts[2], out ErrorType type) == false
            ? throw new ArgumentException("Invalid serialized format")
            : new Error(parts[0], parts[1], type);
    }

    public ErrorList ToErrorList()
    {
        return new([this]);
    }
}