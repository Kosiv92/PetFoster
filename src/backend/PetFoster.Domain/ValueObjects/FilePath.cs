using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record FilePath
    {
        private FilePath(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static Result<FilePath, Error> Create(Guid path, string extension)
        {
            var fullPath = String.Concat(path, '.', extension);

            return new FilePath(fullPath);
        }

        public static Result<FilePath, Error> Create(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return Errors.General.ValueIsInvalid("Path to file must not be empty");

            return new FilePath(fullPath);
        }
    }
}
