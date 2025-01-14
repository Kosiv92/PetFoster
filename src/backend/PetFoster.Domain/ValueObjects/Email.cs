using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;
using System.Text.RegularExpressions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class Email : ComparableValueObject
    {
        public const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public const int MAX_EMAIL_LENGTH = 320;
        public const string COLUMN_NAME = "email";

        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Email, Error> Create(string value)
         => IsValidValue(value)
            ? new Email(value)
            : Errors.General.ValueIsInvalid("Incorrect format of email address");

        private static bool IsValidValue(string value)
        => Regex.IsMatch(value, Email.EMAIL_PATTERN) && value.Length <= 320;

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }
}
