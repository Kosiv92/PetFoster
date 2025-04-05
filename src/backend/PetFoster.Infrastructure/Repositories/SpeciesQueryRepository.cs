using Dapper;
using PetFoster.Application.DTO.Specie;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.GetBreeds;
using PetFoster.Application.Species.GetSpecie;
using PetFoster.Application.Species.GetSpecies;
using PetFoster.Domain.Shared;

namespace PetFoster.Infrastructure.Repositories
{
    public class SpeciesQueryRepository : ISpeciesQueryRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public SpeciesQueryRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedList<SpecieDto>> GetAllAsync(GetSpeciesWithPaginationQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@PageSize", query.PageSize);
            parameters.Add("@Offset", (query.Page - 1) * query.PageSize);

            var totalCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM species WHERE is_deleted = false");

            var specieDict = new Dictionary<Guid, SpecieDto>();

            var sql = """
                   SELECT s.id, s.name, b.id, b.name, b.specie_id
                   FROM species AS s
                   LEFT JOIN breeds AS b ON s.id = b.specie_id
                   WHERE s.is_deleted = false
                   ORDER BY s.name LIMIT @PageSize OFFSET @Offset
                   """;

            var queryResult = await connection.QueryAsync<SpecieDto, BreedDto, SpecieDto>(
             sql, 
             (specie, breed) => 
             {
                 if (!specieDict.TryGetValue(specie.Id, out var specieEntry))
                 {
                     specieEntry = specie;
                     specieEntry.Breeds = new List<BreedDto>();
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

        public async Task<SpecieDto> GetByIdAsync(GetSpecieByIdQuery query, CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@SpecieId", query.Id);

            var specieSqlQuery = """
                   SELECT id, name
                   FROM species                   
                   WHERE id = @SpecieId AND is_deleted = false
                   """;

            var breedsSqlQuery = """
                   SELECT id, name, specie_id
                   FROM breeds                   
                   WHERE specie_id = @SpecieId AND is_deleted = false
                   """;

            var specieQueryResult = connection.QueryFirstOrDefault<SpecieDto>(
             specieSqlQuery, new { SpecieId = query.Id });

            if (specieQueryResult == null) return specieQueryResult;

            var breedsQueryResult = await connection.QueryAsync<BreedDto>(
             breedsSqlQuery, param: parameters);

            specieQueryResult.Breeds = breedsQueryResult.ToList();

            return specieQueryResult;
        }

        public async Task<List<BreedDto>> GetBreedsBySpecieIdAsync(GetBreedsBySpecieIdQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@SpecieId", query.SpecieId);

            var sql = """
                   SELECT id, name, specie_id
                   FROM breeds
                   WHERE specie_id = @SpecieId AND is_deleted = false                   
                   """;

            var queryResult = await connection.QueryAsync<BreedDto>(
             sql, param: parameters);

            return queryResult?.ToList();
        }
    }
}
