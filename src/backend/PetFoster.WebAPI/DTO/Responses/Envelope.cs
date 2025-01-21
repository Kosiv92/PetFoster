namespace PetFoster.WebAPI.DTO.Responses
{
    public sealed record Envelope
    {
        private Envelope(object? result, ErrorList? errors)
        {
            Result = result;
            Errors = errors;
            TimeGenerated = DateTime.Now;
        }

        public object? Result { get; }

        public ErrorList? Errors { get; }

        public DateTime TimeGenerated { get; }

        public static Envelope Ok(object? result = null)
            => new(result, null);

        public static Envelope Error(ErrorList? errors)
            => new(null, errors);
    }

    public sealed record ResponseError(string? ErrorCode, 
        string? ErrorMessage, string? InvalidField);
}
