using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.DTO
{
    public sealed record GetFileDto(FilePath FilePath, string BucketName);
}
