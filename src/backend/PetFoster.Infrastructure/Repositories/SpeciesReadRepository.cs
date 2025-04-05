using Microsoft.EntityFrameworkCore;
using PetFoster.Application.DTO.Specie;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.GetBreeds;
using PetFoster.Application.Species.GetSpecie;
using PetFoster.Application.Species.GetSpecies;
using PetFoster.Domain.Shared;
using PetFoster.Infrastructure.DbContexts;

namespace PetFoster.Infrastructure.Repositories
{
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
            var totalCount = await _dbContext.Species.CountAsync(cancellationToken);

            var species = await _dbContext.Species
                .Skip((query.Page -1) * query.PageSize)
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
}
