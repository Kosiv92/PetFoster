using CSharpFunctionalExtensions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.Volunteers.Application.Files;

namespace PetFoster.Volunteers.Application.Interfaces;

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
