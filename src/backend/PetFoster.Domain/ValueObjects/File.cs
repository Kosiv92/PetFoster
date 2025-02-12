using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public record File
    {
        private File(string pathToStorage)
        {
            PathToStorage = pathToStorage;
        }

        public string PathToStorage { get; }

        public static Result<File, Error> Create(string pathToStorage)
        {
            if (string.IsNullOrWhiteSpace(pathToStorage))
                return Errors.General.ValueIsInvalid("Path to storage must not be empty");

            return new File(pathToStorage);
        }
    }
}
