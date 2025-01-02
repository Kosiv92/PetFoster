using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record Email
    {
        public const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public const int MAX_EMAIL_LENGTH = 320;
        public const string COLUMN_NAME = "email";

        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Email> Create(string value)
         => IsValidValue(value)
            ? Result.Success<Email>(new Email(value))
            : Result.Failure<Email>("Incorrect format of email address");

        private static bool IsValidValue(string value)
        => Regex.IsMatch(value, Email.EMAIL_PATTERN) && value.Length <= 320;
    }
}
