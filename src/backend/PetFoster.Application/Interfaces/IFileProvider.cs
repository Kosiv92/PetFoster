using CSharpFunctionalExtensions;
using PetFoster.Application.DTO;
using PetFoster.Application.FileProvider;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Interfaces
{
    public interface IFileProvider
    {
        public Task<Result<FilePath, Error>> UploadFile(
        FileData fileData, CancellationToken cancellationToken = default);

        public Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

        public Task<UnitResult<Error>> RemoveFile(RemoveFileDto dto,
            CancellationToken cancellationToken = default);

        public Task<Result<string, Error>> GetFileLink(GetFileDto dto,
            CancellationToken cancellationToken = default);
    }
}
