using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;
using System.Text.Json.Serialization;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class AssistanceRequisites : ComparableValueObject
    {
        public const int MAX_NAME_LENGTH = 100;

        [JsonConstructorAttribute]
        private AssistanceRequisites(string name, Description description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public Description Description { get; }

        public static Result<AssistanceRequisites, Error> Create(string name, Description description)
        {
            if (String.IsNullOrWhiteSpace(name) || name.Length > AssistanceRequisites.MAX_NAME_LENGTH)
                return Errors.General.ValueIsInvalid(
                    $"The name cannot be empty or contain more than {AssistanceRequisites.MAX_NAME_LENGTH} characters");

            return new AssistanceRequisites(name, description);
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Name;
            yield return Description;
        }
    }
}
