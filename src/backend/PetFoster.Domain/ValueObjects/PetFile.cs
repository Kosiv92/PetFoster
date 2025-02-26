namespace PetFoster.Domain.ValueObjects
{
    public sealed record PetFile
    {
        public PetFile(FilePath pathToStorage)
        {
            PathToStorage = pathToStorage;
        }

        public FilePath PathToStorage { get; }
    }
}
