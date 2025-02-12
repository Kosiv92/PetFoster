using Microsoft.AspNetCore.Mvc;
using Minio;

namespace PetFoster.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IMinioClient _minioClient;

        public FileController(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }
    }
}
