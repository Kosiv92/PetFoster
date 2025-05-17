using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFoster.Application.Files;
using PetFoster.Application.Interfaces;
using PetFoster.Core;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Infrastructure.Providers
{
    public class MinioProvider : IFileProvider
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 10;

        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioProvider> _logger;
        public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
        {
            _minioClient = minioClient;
            _logger = logger;
        }

        public async Task<Result<FilePath, Error>> UploadFile(
        FileData fileData, CancellationToken cancellationToken = default)
        {
            Result<IReadOnlyList<FilePath>, Error> result = await UploadFiles([fileData], cancellationToken);
            return result.IsFailure ? (Result<FilePath, Error>)result.Error : (Result<FilePath, Error>)result.Value.Single();
        }

        public async Task<UnitResult<Error>> RemoveFile(Application.Files.FileInfo fileInfo,
            CancellationToken cancellationToken = default)
        {
            bool bucketExist = await IsBucketExist(fileInfo.BucketName, cancellationToken);

            if (bucketExist == false)
            {
                return Error.Failure("bucket.not.exist", $"Bucket {fileInfo.BucketName} is not exist");
            }

            try
            {
                StatObjectArgs statArgs = new StatObjectArgs()
                    .WithBucket(fileInfo.BucketName)
                    .WithObject(fileInfo.FilePath.Path);

                Minio.DataModel.ObjectStat? objectStat = await _minioClient.StatObjectAsync(statArgs, cancellationToken);

                if (objectStat is null)
                {
                    return Result.Success<Error>();
                }

                RemoveObjectArgs removeArgs = new RemoveObjectArgs()
                .WithBucket(fileInfo.BucketName)
                .WithObject(fileInfo.FilePath.Path);

                await _minioClient.RemoveObjectAsync(removeArgs, cancellationToken);

                return Result.Success<Error>();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Throw exception {Exception} while remove file {FilePath} from bucket {BucketName} in minio",
                    ex.Message, fileInfo.FilePath.Path, fileInfo.BucketName);

                return Error.Failure("file.remove", "Fail to remove file in minio");
            }
        }

        public async Task<Result<string, Error>> GetFileLink(GetFileDto dto,
            CancellationToken cancellationToken = default)
        {
            bool bucketExist = await IsBucketExist(dto.BucketName, cancellationToken);

            if (bucketExist == false)
            {
                return Error.Failure("bucket.not.exist", $"Bucket {dto.BucketName} is not exist");
            }

            int expiry = (int)TimeSpan.FromHours(1).TotalSeconds;

            try
            {
                PresignedGetObjectArgs getArgs = new PresignedGetObjectArgs()
                .WithBucket(dto.BucketName)
                .WithObject(dto.FilePath.Path)
                .WithExpiry(expiry);

                return await _minioClient.PresignedGetObjectAsync(getArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Throw exception {Exception} while get link for file {FilePath} from bucket {BucketName} in minio",
                    ex.Message, dto.FilePath.Path, dto.BucketName);

                return Error.Failure("file.get.link", $"Fail to get link to file {dto.FilePath.Path} in minio");
            }
        }

        public async Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default)
        {
            SemaphoreSlim semaphoreSlim = new(MAX_DEGREE_OF_PARALLELISM);
            List<FileData> filesList = filesData.ToList();

            try
            {
                await IfBucketsNotExistCreateBucket(filesList, cancellationToken);

                IEnumerable<Task<Result<FilePath, Error>>> tasks = filesList.Select(async file =>
                    await PutObject(file, semaphoreSlim, cancellationToken));

                Result<FilePath, Error>[] pathsResult = await Task.WhenAll(tasks);

                if (pathsResult.Any(p => p.IsFailure))
                {
                    return pathsResult.First().Error;
                }

                List<FilePath> results = pathsResult.Select(p => p.Value).ToList();

                _logger.LogInformation("Uploaded files: {files}", results.Select(f => f.Path));

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Fail to upload files in minio, files amount: {amount}", filesList.Count);

                return Error.Failure("file.upload", "Fail to upload files in minio");
            }
        }

        private async Task<Result<FilePath, Error>> PutObject(
            FileData fileData,
            SemaphoreSlim semaphoreSlim,
            CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync(cancellationToken);

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                .WithBucket(fileData.FileInfo.BucketName)
                .WithStreamData(fileData.Stream)
                .WithObjectSize(fileData.Stream.Length)
                .WithObject(fileData.FileInfo.FilePath.Path);

            try
            {
                _ = await _minioClient
                    .PutObjectAsync(putObjectArgs, cancellationToken);

                return fileData.FileInfo.FilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Throw exception {Exception} while upload files in minio with path {path} in bucket {bucket}",
                    ex.Message,
                    fileData.FileInfo.FilePath.Path,
                    fileData.FileInfo.BucketName);

                return Error.Failure("file.upload", "Fail to upload file in minio");
            }
            finally
            {
                _ = semaphoreSlim.Release();
            }
        }

        private async Task IfBucketsNotExistCreateBucket(
            IEnumerable<FileData> filesData,
            CancellationToken cancellationToken)
        {
            HashSet<string> bucketNames = [.. filesData.Select(file => file.FileInfo.BucketName)];

            foreach (string bucketName in bucketNames)
            {
                bool bucketExist = await IsBucketExist(bucketName, cancellationToken);

                if (bucketExist == false)
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);

                    await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                }
            }
        }

        private Task<bool> IsBucketExist(string bucketName,
            CancellationToken cancellationToken)
        {
            BucketExistsArgs bucketExistArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

            return _minioClient
                .BucketExistsAsync(bucketExistArgs, cancellationToken);
        }
    }
}
