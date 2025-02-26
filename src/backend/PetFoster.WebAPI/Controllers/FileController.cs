using Microsoft.AspNetCore.Mvc;
using PetFoster.Application.DTO;
using PetFoster.Application.FileProvider;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.ValueObjects;
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
            var filePath = FilePath.Create(file).Value;

            var dto = new GetFileDto(filePath, BUCKET_NAME);

            var result = await _fileProvider.GetFileLink(dto, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string file,
            CancellationToken cancellationToken = default)
        {
            var filePath = FilePath.Create(file).Value;

            var dto = new RemoveFileDto(filePath, BUCKET_NAME);

            var result = await _fileProvider.RemoveFile(dto, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] IFormFileCollection files, 
            CancellationToken cancellationToken = default)
        {
            await using var fileProcessor = new FormFileProcessor();
            var fileDtos = fileProcessor.Process(files);

            List<FileData> filesData = new();

            foreach (var file in fileDtos)
            {
                var filePath = FilePath.Create(Guid.NewGuid(), Path.GetExtension(file.FileName));

                var fileData = new FileData(file.Content, filePath.Value, BUCKET_NAME);

                filesData.Add(fileData);
            }

            var result = await _fileProvider.UploadFiles(filesData, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }
    }
}
