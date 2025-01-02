using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record PetName
    {
        public const int MIN_NAME_LENGTH = 1;
        public const int MAX_NAME_LENGTH = 100;

        private PetName() { }

        private PetName(string value)
            => Value = value;

        public string Value { get; }

        public static Result<PetName> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value)
                || value.Length > PetName.MAX_NAME_LENGTH
                || value.Length < MIN_NAME_LENGTH)
                return Result.Failure<PetName>(
                    NotificationFactory.GetErrorForNonNullableValueWithRange("pet name", MIN_NAME_LENGTH, MAX_NAME_LENGTH));

            return Result.Success<PetName>(new PetName(value));
        }
    }
}
