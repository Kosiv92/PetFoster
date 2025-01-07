using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record SpeciesName
    {
        public const int MIN_NAME_LENGTH = 2;
        public const int MAX_NAME_LENGTH = 200;

        private SpeciesName() { }
                
        public SpeciesName(string value) => Value = value;

        public string Value { get; }

        public static Result<SpeciesName> Create(string value)
        {
            if(String.IsNullOrWhiteSpace(value) 
                || value.Length > MAX_NAME_LENGTH 
                || value.Length < MIN_NAME_LENGTH)
            {
                return Result.Failure<SpeciesName>(NotificationFactory
                    .GetErrorForNonNullableValueWithRange(nameof(SpeciesName), MIN_NAME_LENGTH, MAX_NAME_LENGTH));
            }

            return Result.Success<SpeciesName>(new SpeciesName(value));
        }
    }    
}
