using CSharpFunctionalExtensions;
using PetFoster.Application.FileProvider;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Interfaces
{
    public interface IFileProvider
    {
        Task<Result<string, Error>> UploadFile(
        FileData fileData, CancellationToken cancellationToken = default);
    }
}
