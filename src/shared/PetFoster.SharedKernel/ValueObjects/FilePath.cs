using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects;

public sealed record FilePath
{
    private FilePath()
    { }

    private FilePath(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public static Result<FilePath, Error> Create(Guid path, string extension)
    {
        string fullPath = string.Concat(path, extension);

        return new FilePath(fullPath);
    }

    public static Result<FilePath, Error> Create(string fullPath)
    {
        return string.IsNullOrWhiteSpace(fullPath)
            ? (Result<FilePath, Error>)Errors.General.ValueIsInvalid("Path to file must not be empty")
            : (Result<FilePath, Error>)new FilePath(fullPath);
    }
}
