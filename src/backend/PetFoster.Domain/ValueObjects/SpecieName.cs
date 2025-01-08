using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record SpecieName
    {
        public const int MIN_NAME_LENGTH = 2;
        public const int MAX_NAME_LENGTH = 200;

        private SpecieName() { }
                
        public SpecieName(string value) => Value = value;

        public string Value { get; }

        public static Result<SpecieName> Create(string value)
        {
            if(String.IsNullOrWhiteSpace(value) 
                || value.Length > MAX_NAME_LENGTH 
                || value.Length < MIN_NAME_LENGTH)
            {
                return Result.Failure<SpecieName>(NotificationFactory
                    .GetErrorForNonNullableValueWithRange(nameof(SpecieName), MIN_NAME_LENGTH, MAX_NAME_LENGTH));
            }

            return Result.Success<SpecieName>(new SpecieName(value));
        }
    }    
}
