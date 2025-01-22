namespace PetFoster.Domain.Shared
{
    public record Error
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
            => new Error(code, message, ErrorType.Validation, invalidField);

        public static Error NotFound(string code, string message)
            => new Error(code, message, ErrorType.NotFound);

        public static Error Failure(string code, string message)
            => new Error(code, message, ErrorType.Failure);

        public static Error Conflict(string code, string message)
            => new Error(code, message, ErrorType.Conflict);

        public string Serialize() 
            => string.Join(Error.SEPARATOR, this.Code, this.Message, this.Type);

        public static Error Deserialize(string serialized)
        {
            var parts = serialized.Split(Error.SEPARATOR);
            if (parts.Length < 3) 
                throw new ArgumentException("Invalid serialized format");

            if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
                throw new ArgumentException("Invalid serialized format");

            return new Error(parts[0], parts[1], type);
        }

        public ErrorList ToErrorList() => new([this]);
    }        
}

public enum ErrorType
{
    Validation = 0, 
    NotFound = 1, 
    Failure = 2, 
    Conflict = 3
}
