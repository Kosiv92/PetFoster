using Microsoft.EntityFrameworkCore;
using PetFoster.Core.DTO.Specie;
using PetFoster.Core.Models;
using PetFoster.Species.Application.BreedManagement.GetBreeds;
using PetFoster.Species.Application.Interfaces;
using PetFoster.Species.Application.SpecieManagement.GetSpecie;
using PetFoster.Species.Application.SpecieManagement.GetSpecies;
using PetFoster.Species.Infrastructure.DbContexts;

namespace PetFoster.Species.Infrastructure.Repositories;

public class SpeciesReadRepository : ISpeciesQueryRepository
{
    private readonly ReadDbContext _dbContext;

    public SpeciesReadRepository(ReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedList<SpecieDto>> GetAllAsync(GetSpeciesWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        int totalCount = await _dbContext.Species.CountAsync(cancellationToken);

        List<SpecieDto> species = await _dbContext.Species
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize).ToListAsync(cancellationToken);

        return new PagedList<SpecieDto>()
        {
            Items = species,
            TotalCount = totalCount,
            PageSize = query.PageSize,
            Page = query.Page,
        };
    }

    public Task<SpecieDto> GetByIdAsync(GetSpecieByIdQuery query,
        CancellationToken cancellationToken)
    {
        return _dbContext.Species.Include(s => s.Breeds)
            .FirstOrDefaultAsync(s => s.Id == query.Id);
    }

    public Task<List<BreedDto>> GetBreedsBySpecieIdAsync(GetBreedsBySpecieIdQuery query,
        CancellationToken cancellationToken)
    {
        return _dbContext.Breeds
            .Where(b => b.SpecieId == query.SpecieId)
            .ToListAsync(cancellationToken);
    }
}
