using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NSubstitute;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.Volunteers.Application.Files;
using PetFoster.Volunteers.Application.Interfaces;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace PetFoster.IntegrateTests;

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

        await ApplyMigrationsAsync(scope);

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());

        await InitializeRespawner();
    }

    private async Task ApplyMigrationsAsync(IServiceScope scope)
    {        
        await ApplyDbContextMigrations<PetFoster.Volunteers.Infrastructure.DbContexts.WriteDbContext>(scope);
        await ApplyDbContextMigrations<PetFoster.Species.Infrastructure.DbContexts.WriteDbContext>(scope);
    }

    private async Task ApplyDbContextMigrations<TDbContext>(IServiceScope scope) where TDbContext : DbContext
    {
        //var options = new DbContextOptionsBuilder<TDbContext>()
        //    .UseNpgsql(_dbContainer.GetConnectionString())
        //    .Options;

        //await using var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), options);

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    //private async Task EnsureCreatedDatabase(IServiceScope scope)
    //{
    //    PetFoster.Volunteers.Infrastructure.DbContexts.WriteDbContext volunteerDbContext 
    //        = scope.ServiceProvider.GetRequiredService<PetFoster.Volunteers.Infrastructure.DbContexts.WriteDbContext>();
    //    PetFoster.Species.Infrastructure.DbContexts.WriteDbContext speciesDbContext
    //        = scope.ServiceProvider.GetRequiredService<PetFoster.Species.Infrastructure.DbContexts.WriteDbContext>();

    //    //var delVol = await volunteerDbContext.Database.EnsureDeletedAsync();
    //    var delSpe = await speciesDbContext.Database.EnsureDeletedAsync();

    //    //var creVol = await volunteerDbContext.Database.EnsureCreatedAsync();
    //    var creSpe = await speciesDbContext.Database.EnsureCreatedAsync();
    //}

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
        builder.ConfigureTestServices(ConfigureDefaultServices);
        builder.ConfigureAppConfiguration((context, config) =>
       {
           config.AddInMemoryCollection(new Dictionary<string, string>
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
            services.Remove(fileSerice);
        }

        services.AddTransient<IFileProvider>(_ => _fileProviderMock);
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

        _fileProviderMock.UploadFiles(Arg.Any<IEnumerable<FileData>>(),
           Arg.Any<CancellationToken>())
           .Returns(Result.Success<IReadOnlyList<FilePath>, Error>(response));
    }

    public void SetupFailureFileProviderMock()
    {
        _fileProviderMock.UploadFiles(Arg.Any<IEnumerable<FileData>>(),
           Arg.Any<CancellationToken>())
           .Returns(Result.Failure<IReadOnlyList<FilePath>, Error>(
               Error.Failure("upload.file.failed", "Failed to upload files")));
    }
}
