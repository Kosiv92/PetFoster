using Dapper;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetPetByID;
using PetFoster.Application.Volunteers.GetPets;
using PetFoster.Application.Volunteers.GetPetsByBreedId;
using PetFoster.Application.Volunteers.GetPetsBySpecieId;
using PetFoster.Application.Volunteers.GetPetsByVolunteerId;
using PetFoster.Application.Volunteers.GetVolunteer;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;
using System.Text;
using System.Text.Json;

namespace PetFoster.Infrastructure.Repositories
{
    public sealed class VolunteersQueryRepository : IVolunteersQueryRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        const string DESC = "DESC";

        public VolunteersQueryRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedList<VolunteerDto>> GetAllAsync(GetVoluteersWithPaginationQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@PageSize", query.PageSize);
            parameters.Add("@Offset", (query.Page - 1) * query.PageSize);

            var totalCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM volunteers");

            var sql = """
                   SELECT id, first_name, last_name, patronymic, email, description, work_expirience, 
                   phone_number, is_deleted, assistance_requisites_list, social_net_contacts 
                   FROM volunteers
                   WHERE is_deleted = false
                   ORDER BY last_name,first_name,patronymic LIMIT @PageSize OFFSET @Offset
                   """;

            var queryResult = await connection
                .QueryAsync<VolunteerDto, string, string, VolunteerDto>(
             sql,
             (volunteer, assistanseJson, socialJson) =>
             {
                 var assistanceRequisites = JsonSerializer
                 .Deserialize<List<AssistanceRequisites>>(assistanseJson) ?? new();

                 var socialContacts = JsonSerializer
                 .Deserialize<List<SocialNetContact>>(socialJson) ?? new();

                 volunteer.AssistanceRequisitesList = assistanceRequisites
                    .Select(a => new AssistanceRequisitesDto(a.Name, a.Description?.Value))
                    .ToList();

                 volunteer.SocialNetContacts = socialContacts
                    .Select(s => new SocialNetContactsDto(s.SocialNetName, s.AccountName))
                    .ToList();

                 return volunteer;
             },
             splitOn: "assistance_requisites_list, social_net_contacts",
             param: parameters);

            return new PagedList<VolunteerDto>()
            {
                Items = queryResult.ToList(),
                TotalCount = totalCount,
                PageSize = query.PageSize,
                Page = query.Page,
            };

        }

        public async Task<PagedList<PetDto>> GetPetWithPaginationAsync(
            GetPetsWithPaginationQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@PageSize", query.PageSize);
            parameters.Add("@Offset", (query.Page - 1) * query.PageSize);

            var totalCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM pets");

            StringBuilder sqlBuilder = new StringBuilder();                        

            var sqlSelectPart = """
                   SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
                   сastrated, birth_day, vaccinated, position, assistance_status, file_list
                   FROM pets
                   """;

            sqlBuilder.Append(sqlSelectPart);

            if (query.FilterList?.Any() == true)
            {
                var filters = new List<string>();

                int currentIndex = 1;

                foreach (var filter in query.FilterList)
                {
                    filters.Add($"{filter.FilterPropertyName} {filter.FilterCondition} @FilterValue{currentIndex}");
                    parameters.Add($"@FilterValue{currentIndex}", filter.FilterValue);
                    currentIndex++;
                }

                var filterSqlPart = String.Concat(" WHERE ", String.Join(" AND ", filters));

                sqlBuilder.AppendLine(filterSqlPart);
            }

            if (query.SortBy != null)
            {      
                var orderDirection = query.SortAsc ? String.Empty : "DESC";
                sqlBuilder.AppendLine($" ORDER BY {query.SortBy} {orderDirection}");
            }

            sqlBuilder.AppendLine("LIMIT @PageSize OFFSET @Offset");

            var sqlQuery = sqlBuilder.ToString();

            var queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
             sqlQuery,
             (pet, petFilesJson) =>
             {
                 var petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? new();

                 pet.PetFiles = petFiles
                    .Select(f => new PetFileDto(f.PathToStorage.Path))
                    .ToList();

                 return pet;
             },
             splitOn: "file_list",
             param: parameters);

            return new PagedList<PetDto>()
            {
                Items = queryResult.ToList(),
                TotalCount = totalCount,
                PageSize = query.PageSize,
                Page = query.Page,
            };
        }

        public async Task<VolunteerDto> GetByIdAsync(GetVolunteerByIdQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@VolunteerId", query.Id);

            var sql = """
                   SELECT id, first_name, last_name, patronymic, email, description, work_expirience, 
                    phone_number, is_deleted, assistance_requisites_list, social_net_contacts 
                   FROM volunteers
                   WHERE id = @VolunteerId AND is_deleted = false
                   """;

            var queryResult = await connection.QueryAsync<VolunteerDto, string, string, VolunteerDto>(
             sql,
             (volunteer, assistanseJson, socialJson) =>
             {
                 var assistanceRequisites = JsonSerializer
                 .Deserialize<List<AssistanceRequisites>>(assistanseJson) ?? new();

                 var socialContacts = JsonSerializer
                 .Deserialize<List<SocialNetContact>>(socialJson) ?? new();

                 volunteer.AssistanceRequisitesList = assistanceRequisites
                    .Select(a => new AssistanceRequisitesDto(a.Name, a.Description?.Value))
                    .ToList();

                 volunteer.SocialNetContacts = socialContacts
                    .Select(s => new SocialNetContactsDto(s.SocialNetName, s.AccountName))
                    .ToList();

                 return volunteer;
             },
             splitOn: "assistance_requisites_list, social_net_contacts",
             param: parameters);

            return queryResult.FirstOrDefault();
        }

        public async Task<PetDto> GetPetByIdAsync(GetPetByIdQuery query,
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@VolunteerId", query.VolunteerId);
            parameters.Add("@PetId", query.PetId);

            var sql = """
                   SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
                   сastrated, birth_day, vaccinated, position, assistance_status, file_list 
                   FROM pets
                   WHERE volunteer_id = @VolunteerId AND id = @PetId AND is_deleted = false
                   """;

            var queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
             sql,
             (pet, petFilesJson) =>
             {
                 var petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? new();

                 pet.PetFiles = petFiles
                    .Select(f => new PetFileDto(f.PathToStorage.Path))
                    .ToList();

                 return pet;
             },
             splitOn: "file_list",
             param: parameters);

            return queryResult.FirstOrDefault();
        }

        public async Task<IEnumerable<PetDto>> GetPetsByVolunteerId(GetPetsByVolunteerIdQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@VolunteerId", query.VolunteerId);

            var sql = """
                   SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
                   сastrated, birth_day, vaccinated, position, assistance_status, file_list 
                   FROM pets
                   WHERE volunteer_id = @VolunteerId AND is_deleted = false
                   """;

            var queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
             sql,
             (pet, petFilesJson) =>
             {
                 var petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? new();

                 pet.PetFiles = petFiles
                    .Select(f => new PetFileDto(f.PathToStorage.Path))
                    .ToList();

                 return pet;
             },
             splitOn: "file_list",
             param: parameters);

            return queryResult;
        }


        public async Task<IEnumerable<PetDto>> GetPetsBySpecieId(GetPetsBySpecieIdQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@SpecieId", query.SpecieId.Value);

            var sql = """
                   SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
                   сastrated, birth_day, vaccinated, position, assistance_status, file_list
                   FROM pets
                   WHERE specie_id = @SpecieId AND is_deleted = false
                   """;

            var queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
             sql,
             (pet, petFilesJson) =>
             {
                 var petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? new();

                 pet.PetFiles = petFiles
                    .Select(f => new PetFileDto(f.PathToStorage.Path))
                    .ToList();

                 return pet;
             },
             splitOn: "file_list",
             param: parameters);

            return queryResult;
        }

        public async Task<IEnumerable<PetDto>> GetPetsByBreedId(GetPetsByBreedIdQuery query, 
            CancellationToken cancellationToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@BreedId", query.BreedId.Value);

            var sql = """
                   SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
                   сastrated, birth_day, vaccinated, position, assistance_status, file_list
                   FROM pets
                   WHERE breed_id = @BreedId AND is_deleted = false
                   """;

            var queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
             sql,
             (pet, petFilesJson) =>
             {
                 var petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? new();

                 pet.PetFiles = petFiles
                    .Select(f => new PetFileDto(f.PathToStorage.Path))
                    .ToList();

                 return pet;
             },
             splitOn: "file_list",
             param: parameters);

            return queryResult;
        }               
    }
}
