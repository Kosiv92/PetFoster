using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Files
{
    public sealed record FileInfo(FilePath FilePath, string BucketName);
}


