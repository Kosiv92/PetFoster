using CSharpFunctionalExtensions;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Files;
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

        public Task<UnitResult<Error>> RemoveFile(Files.FileInfo fileInfo,
            CancellationToken cancellationToken = default);

        public Task<Result<string, Error>> GetFileLink(GetFileDto dto,
            CancellationToken cancellationToken = default);
    }
}
