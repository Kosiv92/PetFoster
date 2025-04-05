using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.DTO.Volunteer
{
    public sealed record GetFileDto(FilePath FilePath, string BucketName);
}
