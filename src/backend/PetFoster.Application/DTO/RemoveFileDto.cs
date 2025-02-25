using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.DTO
{
    public sealed record RemoveFileDto(FilePath FilePath, string BucketName);    
}
