using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.DTO.Volunteer
{
    public sealed record RemoveFileDto(FilePath FilePath, string BucketName);
}
