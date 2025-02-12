using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio.DataModel.Args;
using Minio;
using PetFoster.Application.FileProvider;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Infrastructure.Providers
{
    public class MinioProvider : IFileProvider
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioProvider> _logger;
        public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
        {
            _minioClient = minioClient;
            _logger = logger;
        }
        public async Task<Result<string, Error>> UploadFile(
            FileData fileData, CancellationToken cancellationToken = default)
        {
            try
            {
                var bucketExistArgs = new BucketExistsArgs()
                    .WithBucket("photos");
                var bucketExist = await _minioClient.BucketExistsAsync(bucketExistArgs, cancellationToken);
                if (bucketExist == false)
                {
                    var makeBucketArgs = new MakeBucketArgs()
                        .WithBucket("photos");
                    await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                }
                var path = Guid.NewGuid();
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket("photos")
                    .WithStreamData(fileData.Stream)
                    .WithObjectSize(fileData.Stream.Length)
                    .WithObject(path.ToString());
                var result = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
                return result.ObjectName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to upload file in minio");
                return Error.Failure("file.upload", "Fail to upload file in minio");
            }
        }
    }
}
