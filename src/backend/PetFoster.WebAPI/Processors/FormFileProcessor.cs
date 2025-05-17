using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.WebAPI.Processors
{
    public sealed class FormFileProcessor : IAsyncDisposable
    {
        private readonly List<UploadFileDto> _fileDtos = [];

        public List<UploadFileDto> Process(IFormFileCollection files)
        {
            foreach (IFormFile file in files)
            {
                Stream stream = file.OpenReadStream();
                UploadFileDto fileDto = new(stream, file.FileName);
                _fileDtos.Add(fileDto);
            }

            return _fileDtos;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (UploadFileDto file in _fileDtos)
            {
                await file.Content.DisposeAsync();
            }
        }
    }
}
