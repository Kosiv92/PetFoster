using Microsoft.AspNetCore.Mvc;
using PetFoster.Application.Files;
using PetFoster.Application.Interfaces;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;
using PetFoster.WebAPI.Extensions;
using PetFoster.WebAPI.Processors;

namespace PetFoster.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private const string BUCKET_NAME = "test";

        private readonly IFileProvider _fileProvider;

        public FileController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string file,
            CancellationToken cancellationToken = default)
        {
            FilePath filePath = FilePath.Create(file).Value;

            GetFileDto dto = new(filePath, BUCKET_NAME);

            CSharpFunctionalExtensions.Result<string, Core.Error> result = await _fileProvider.GetFileLink(dto, cancellationToken);

            return result.IsFailure ? result.Error.ToResponse() : (IActionResult)Ok(result.Value);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string file,
            CancellationToken cancellationToken = default)
        {
            FilePath filePath = FilePath.Create(file).Value;

            Application.Files.FileInfo fileInfo = new(filePath, BUCKET_NAME);

            CSharpFunctionalExtensions.UnitResult<Core.Error> result = await _fileProvider.RemoveFile(fileInfo, cancellationToken);

            return result.IsFailure ? result.Error.ToResponse() : (IActionResult)Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] IFormFileCollection files,
            CancellationToken cancellationToken = default)
        {
            await using FormFileProcessor fileProcessor = new();
            List<UploadFileDto> fileDtos = fileProcessor.Process(files);

            List<FileData> filesData = [];

            foreach (UploadFileDto file in fileDtos)
            {
                CSharpFunctionalExtensions.Result<FilePath, Core.Error> filePath = FilePath.Create(Guid.NewGuid(), Path.GetExtension(file.FileName));

                FileData fileData = new(file.Content, new Application.Files.FileInfo(filePath.Value, BUCKET_NAME));

                filesData.Add(fileData);
            }

            CSharpFunctionalExtensions.Result<IReadOnlyList<FilePath>, Core.Error> result = await _fileProvider.UploadFiles(filesData, cancellationToken);

            return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
        }
    }
}
