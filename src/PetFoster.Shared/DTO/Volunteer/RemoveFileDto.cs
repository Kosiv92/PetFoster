using PetFoster.Core.ValueObjects;

namespace PetFoster.Core.DTO.Volunteer
{
    public sealed record RemoveFileDto(FilePath FilePath, string BucketName);
}
