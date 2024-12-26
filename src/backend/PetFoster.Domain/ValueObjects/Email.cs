using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record Email
    {
        private const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        private Email(string value)
        {
            Value = value;
        }

        string Value { get; }

        public static Result<Email> Create(string value)
         => IsValidValue(value)
            ? Result.Success<Email>(new Email(value))
            : Result.Failure<Email>("Incorrect format of email address");

        private static bool IsValidValue(string value)
        => Regex.IsMatch(value, emailPattern);
    }
}
