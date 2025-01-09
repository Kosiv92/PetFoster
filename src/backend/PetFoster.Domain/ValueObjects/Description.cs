using CSharpFunctionalExtensions;

namespace PetFoster.Domain
{
    public sealed class Description : ComparableValueObject
    {
        public const int MAX_DESCRIPTION_LENGTH = 500;

        private Description(string? value) => Value = value;

        public string? Value { get; }

        public static Result<Description> Create(string? value)
        {
            if(value != null && value.Length > MAX_DESCRIPTION_LENGTH) 
                return Result.Failure<Description>(
                    $"Description cannot contain more than {Description.MAX_DESCRIPTION_LENGTH} characters");
        
            return Result.Success<Description>(new Description(value)) ; 
        }

        public static Result<Description> Empty() => Description.Create(null);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }
    
}
