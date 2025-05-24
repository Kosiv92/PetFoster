using Dapper;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Models;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Application.PetManagement.GetPetById;
using PetFoster.Volunteers.Application.PetManagement.GetPets;
using PetFoster.Volunteers.Application.PetManagement.GetPetsByBreedId;
using PetFoster.Volunteers.Application.PetManagement.GetPetsBySpecieId;
using PetFoster.Volunteers.Application.PetManagement.GetPetsByVolunteerId;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;
using System.Text;
using System.Text.Json;

namespace PetFoster.Volunteers.Infrastructure.Repositories;

public sealed class VolunteersQueryRepository : IVolunteersQueryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private const string DESC = "DESC";

    public VolunteersQueryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedList<VolunteerDto>> GetAllAsync(GetVoluteersWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@PageSize", query.PageSize);
        parameters.Add("@Offset", (query.Page - 1) * query.PageSize);

        long totalCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM volunteers.volunteers");

        string sql = """
               SELECT id, first_name, last_name, patronymic, email, description, work_expirience, 
               phone_number, is_deleted, assistance_requisites_list, social_net_contacts 
               FROM volunteers.volunteers
               WHERE is_deleted = false
               ORDER BY last_name,first_name,patronymic LIMIT @PageSize OFFSET @Offset
               """;

        IEnumerable<VolunteerDto> queryResult = await connection
            .QueryAsync<VolunteerDto, string, string, VolunteerDto>(
         sql,
         (volunteer, assistanseJson, socialJson) =>
         {
             List<AssistanceRequisites> assistanceRequisites = JsonSerializer
             .Deserialize<List<AssistanceRequisites>>(assistanseJson) ?? [];

             List<SocialNetContact> socialContacts = JsonSerializer
             .Deserialize<List<SocialNetContact>>(socialJson) ?? [];

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
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@PageSize", query.PageSize);
        parameters.Add("@Offset", (query.Page - 1) * query.PageSize);

        long totalCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM volunteers.pets");

        StringBuilder sqlBuilder = new();

        string sqlSelectPart = """
               SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
               сastrated, birth_day, vaccinated, position, assistance_status, file_list
               FROM volunteers.pets
               """;

         sqlBuilder.Append(sqlSelectPart);

        if (query.FilterList?.Any() == true)
        {
            List<string> filters = [];

            int currentIndex = 1;

            foreach (FilterItemDto filter in query.FilterList)
            {
                filters.Add($"{filter.FilterPropertyName} {filter.FilterCondition} @FilterValue{currentIndex}");
                parameters.Add($"@FilterValue{currentIndex}", filter.FilterValue);
                currentIndex++;
            }

            string filterSqlPart = string.Concat(" WHERE ", string.Join(" AND ", filters));

             sqlBuilder.AppendLine(filterSqlPart);
        }

        if (query.SortBy != null)
        {
            string orderDirection = query.SortAsc ? string.Empty : "DESC";
             sqlBuilder.AppendLine($" ORDER BY {query.SortBy} {orderDirection}");
        }

         sqlBuilder.AppendLine(" LIMIT @PageSize OFFSET @Offset");

        string sqlQuery = sqlBuilder.ToString();

        IEnumerable<PetDto> queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
         sqlQuery,
         (pet, petFilesJson) =>
         {
             List<PetFile> petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? [];

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
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@VolunteerId", query.Id);

        string sql = """
               SELECT id, first_name, last_name, patronymic, email, description, work_expirience, 
                phone_number, is_deleted, assistance_requisites_list, social_net_contacts 
               FROM volunteers.volunteers
               WHERE id = @VolunteerId AND is_deleted = false
               """;

        IEnumerable<VolunteerDto> queryResult = await connection.QueryAsync<VolunteerDto, string, string, VolunteerDto>(
         sql,
         (volunteer, assistanseJson, socialJson) =>
         {
             List<AssistanceRequisites> assistanceRequisites = JsonSerializer
             .Deserialize<List<AssistanceRequisites>>(assistanseJson) ?? [];

             List<SocialNetContact> socialContacts = JsonSerializer
             .Deserialize<List<SocialNetContact>>(socialJson) ?? [];

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
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@VolunteerId", query.VolunteerId);
        parameters.Add("@PetId", query.PetId);

        string sql = """
               SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
               сastrated, birth_day, vaccinated, position, assistance_status, file_list 
               FROM volunteers.pets
               WHERE volunteer_id = @VolunteerId AND id = @PetId AND is_deleted = false
               """;

        IEnumerable<PetDto> queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
         sql,
         (pet, petFilesJson) =>
         {
             List<PetFile> petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? [];

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
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@VolunteerId", query.VolunteerId);

        string sql = """
               SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
               сastrated, birth_day, vaccinated, position, assistance_status, file_list 
               FROM volunteers.pets
               WHERE volunteer_id = @VolunteerId AND is_deleted = false
               """;

        IEnumerable<PetDto> queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
         sql,
         (pet, petFilesJson) =>
         {
             List<PetFile> petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? [];

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
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@SpecieId", query.SpecieId.Value);

        string sql = """
               SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
               сastrated, birth_day, vaccinated, position, assistance_status, file_list
               FROM volunteers.pets
               WHERE specie_id = @SpecieId AND is_deleted = false
               """;

        IEnumerable<PetDto> queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
         sql,
         (pet, petFilesJson) =>
         {
             List<PetFile> petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? [];

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
        System.Data.IDbConnection connection = _connectionFactory.CreateConnection();
        DynamicParameters parameters = new();

        parameters.Add("@BreedId", query.BreedId.Value);

        string sql = """
               SELECT id, name, specie_id, description, breed_id, coloration, health, weight, height, phone_number, 
               сastrated, birth_day, vaccinated, position, assistance_status, file_list
               FROM volunteers.pets
               WHERE breed_id = @BreedId AND is_deleted = false
               """;

        IEnumerable<PetDto> queryResult = await connection.QueryAsync<PetDto, string, PetDto>(
         sql,
         (pet, petFilesJson) =>
         {
             List<PetFile> petFiles = JsonSerializer.Deserialize<List<PetFile>>(petFilesJson) ?? [];

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
