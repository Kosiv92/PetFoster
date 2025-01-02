using CSharpFunctionalExtensions;
using PetFoster.Domain.Entities;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record AssistanceRequisites
    {
        public const int MAX_NAME_LENGTH = 100;

        public Volunteer Volunteer { get; private set; } = null!;

        private AssistanceRequisites() { }

        private AssistanceRequisites(string name, Description description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public Description Description { get; }

        public static Result<AssistanceRequisites> Create(string name, Description description)
        {
            if (String.IsNullOrWhiteSpace(name) || name.Length > AssistanceRequisites.MAX_NAME_LENGTH)
                return Result.Failure<AssistanceRequisites>(
                    $"The name cannot be empty or contain more than {AssistanceRequisites.MAX_NAME_LENGTH} characters");

            return Result.Success<AssistanceRequisites>(new AssistanceRequisites(name, description));
        }    
    }    
}
