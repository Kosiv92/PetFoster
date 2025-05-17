using PetFoster.Core.ValueObjects;

namespace PetFoster.Core.DTO.Volunteer
{
    public sealed record GetFileDto(FilePath FilePath, string BucketName);
}
