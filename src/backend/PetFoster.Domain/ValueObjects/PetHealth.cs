using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record PetHealth
    {
        public const int MAX_PET_HEALTH_LENGTH = 500;

        private PetHealth() { }

        private PetHealth(string value) => Value = value;

        public string Value { get; }

        public static Result<PetHealth> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length > MAX_PET_HEALTH_LENGTH)
                return Result.Failure<PetHealth>(
                    $"Pet health cannot be empty and contain more than {PetHealth.MAX_PET_HEALTH_LENGTH} characters");

            return Result.Success<PetHealth>(new PetHealth(value));
        }
    }
}
