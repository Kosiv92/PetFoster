using Dapper;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Specie;
using PetFoster.Core.Models;
using PetFoster.Species.Application.BreedManagement.GetBreeds;
using PetFoster.Species.Application.Interfaces;
using PetFoster.Species.Application.SpecieManagement.GetSpecie;
using PetFoster.Species.Application.SpecieManagement.GetSpecies;

namespace PetFoster.Species.Infrastructure.Repositories;

public class SpeciesQueryRepository : ISpeciesQueryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SpeciesQueryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedList<SpecieDto>> GetAllAsync(
        GetSpeciesWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@PageSize", query.PageSize);
        parameters.Add("@Offset", (query.Page - 1) * query.PageSize);

        long totalCount = await connection
            .ExecuteScalarAsync<long>("SELECT COUNT(*) FROM species.species WHERE is_deleted = false");

        Dictionary<Guid, SpecieDto> specieDict = [];

        string sql = """
               SELECT s.id, s.name, b.id, b.name, b.specie_id
               FROM species.species AS s
               LEFT JOIN species.breeds AS b ON s.id = b.specie_id
               WHERE s.is_deleted = false
               ORDER BY s.name LIMIT @PageSize OFFSET @Offset
               """;

        IEnumerable<SpecieDto> queryResult
            = await connection.QueryAsync<SpecieDto, BreedDto, SpecieDto>(
         sql,
         (specie, breed) =>
         {
             if (!specieDict.TryGetValue(specie.Id, out SpecieDto? specieEntry))
             {
                 specieEntry = specie;
                 specieEntry.Breeds = [];
                 specieDict.Add(specieEntry.Id, specieEntry);
             }

             if (breed != null)
             {
                 specieEntry.Breeds.Add(breed);
             }
             return specieEntry;
         },
         param: parameters,
         splitOn: "id");

        return new PagedList<SpecieDto>()
        {
            Items = specieDict.Values.ToList(),
            TotalCount = totalCount,
            PageSize = query.PageSize,
            Page = query.Page,
        };
    }

    public async Task<SpecieDto> GetByIdAsync(
        GetSpecieByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@SpecieId", query.Id);

        string specieSqlQuery = """
               SELECT id, name
               FROM species.species                   
               WHERE id = @SpecieId AND is_deleted = false
               """;

        string breedsSqlQuery = """
               SELECT id, name, specie_id
               FROM species.breeds                   
               WHERE specie_id = @SpecieId AND is_deleted = false
               """;

        SpecieDto? specieQueryResult = connection.QueryFirstOrDefault<SpecieDto>(
         specieSqlQuery, new { SpecieId = query.Id });

        if (specieQueryResult == null)
        {
            return specieQueryResult;
        }

        IEnumerable<BreedDto> breedsQueryResult = await connection.QueryAsync<BreedDto>(
         breedsSqlQuery, param: parameters);

        specieQueryResult.Breeds = breedsQueryResult.ToList();

        return specieQueryResult;
    }

    public async Task<List<BreedDto>> GetBreedsBySpecieIdAsync(
        GetBreedsBySpecieIdQuery query,
        CancellationToken cancellationToken)
    {
        var connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@SpecieId", query.SpecieId);

        string sql = """
               SELECT id, name, specie_id
               FROM species.breeds
               WHERE specie_id = @SpecieId AND is_deleted = false                   
               """;

        IEnumerable<BreedDto> queryResult = await connection.QueryAsync<BreedDto>(
         sql, param: parameters);

        return queryResult?.ToList();
    }
}
