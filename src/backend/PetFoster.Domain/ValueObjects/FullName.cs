using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class FullName : ComparableValueObject
    {
        public const int MIN_NAME_LENGTH = 2;
        public const int MAX_NAME_LENGTH = 2;

        private FullName() { }

        private FullName(string firstName, string lastName, string? patronymic)
        {
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string? Patronymic { get; }

        public static Result<FullName> Create(string firstName, string lastName, string? patronymic)
        {
            if(IsValidNonNullableValue(firstName)) 
                return Result.Failure<FullName>(
                    NotificationFactory.GetErrorForNonNullableValueWithRange(nameof(firstName), MIN_NAME_LENGTH, MAX_NAME_LENGTH));

            if (IsValidNonNullableValue(lastName))
                return Result.Failure<FullName>(
                    NotificationFactory.GetErrorForNonNullableValueWithRange(nameof(lastName), MIN_NAME_LENGTH, MAX_NAME_LENGTH));

            if (IsValidNullableValue(patronymic))
                return Result.Failure<FullName>(
                    NotificationFactory.GetErrorForNonNullableValueWithRange(nameof(patronymic), MIN_NAME_LENGTH, MAX_NAME_LENGTH));

            return Result.Success<FullName>(new FullName(firstName, lastName, patronymic));
        }

        private static bool IsValidNonNullableValue(string property)
            => String.IsNullOrWhiteSpace(property) || property.Length < MIN_NAME_LENGTH || property.Length > MAX_NAME_LENGTH;

        private static bool IsValidNullableValue(string property)
            => String.IsNullOrWhiteSpace(property) || (!String.IsNullOrWhiteSpace(property) && property.Length < MIN_NAME_LENGTH);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return Patronymic;
        }
    };    
}
