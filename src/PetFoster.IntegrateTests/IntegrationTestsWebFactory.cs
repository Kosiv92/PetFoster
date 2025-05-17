using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NSubstitute;
using PetFoster.Application.Files;
using PetFoster.Application.Interfaces;
using PetFoster.Core;
using PetFoster.Core.ValueObjects;
using PetFoster.Infrastructure.DbContexts;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace PetFoster.IntegrateTests
{
    public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly IFileProvider _fileProviderMock = Substitute.For<IFileProvider>();
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("pet_foster_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
        private Respawner _respawner = null!;
        private DbConnection _dbConnection = null!;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            using IServiceScope scope = Services.CreateScope();
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            _ = await dbContext.Database.EnsureDeletedAsync();
            _ = await dbContext.Database.EnsureCreatedAsync();

            _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());

            await InitializeRespawner();
        }

        private async Task InitializeRespawner()
        {
            await _dbConnection.OpenAsync();

            IEnumerable<string> schemas = await _dbConnection.QueryAsync<string>(
                "SELECT schema_name FROM information_schema.schemata WHERE schema_name NOT LIKE 'pg_%' AND schema_name != 'information_schema'");

            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = schemas.ToArray()
            });
        }

        public async Task ResetDataBaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _ = builder.ConfigureTestServices(ConfigureDefaultServices);
            _ = builder.ConfigureAppConfiguration((context, config) =>
            {
                _ = config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:Database", _dbContainer.GetConnectionString() }
                });
            });
        }

        protected virtual void ConfigureDefaultServices(IServiceCollection services)
        {
            ServiceDescriptor? fileSerice = services
                .SingleOrDefault(s => s.ServiceType == typeof(IFileProvider));

            if (fileSerice != null)
            {
                _ = services.Remove(fileSerice);
            }

            _ = services.AddTransient<IFileProvider>(_ => _fileProviderMock);
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }

        public void SetupFileProviderMock(IEnumerable<string> filePathList)
        {
            List<FilePath> response = [];

            foreach (string filePath in filePathList)
            {
                response.Add(FilePath.Create(filePath).Value);
            }

            _ = _fileProviderMock.UploadFiles(Arg.Any<IEnumerable<FileData>>(),
                Arg.Any<CancellationToken>())
                .Returns(Result.Success<IReadOnlyList<FilePath>, Error>(response));
        }

        public void SetupFailureFileProviderMock()
        {
            _ = _fileProviderMock.UploadFiles(Arg.Any<IEnumerable<FileData>>(),
                Arg.Any<CancellationToken>())
                .Returns(Result.Failure<IReadOnlyList<FilePath>, Error>(
                    Error.Failure("upload.file.failed", "Failed to upload files")));
        }
    }
}
