using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.FileProvider
{
    public sealed record FileData(Stream Stream, FilePath FilePath, string BucketName);    
}
