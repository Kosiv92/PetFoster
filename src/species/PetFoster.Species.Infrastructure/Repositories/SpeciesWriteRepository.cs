using Microsoft.EntityFrameworkCore;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Species.Domain.Entities;
using PetFoster.Species.Infrastructure.DbContexts;
using System.Linq.Expressions;

namespace PetFoster.Species.Infrastructure.Repositories;

public sealed class SpeciesWriteRepository : IRepository<Specie, SpecieId>
{
    private readonly WriteDbContext _dbContext;
    private readonly DbSet<Specie> _species;

    public SpeciesWriteRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
        _species = _dbContext.Species;
    }

    public async Task AddAsync(Specie entity, CancellationToken cancellationToken)
    {
         await _species.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(null, cancellationToken);
    }

    public async Task<SpecieId> DeleteAsync(SpecieId id, CancellationToken cancellationToken)
    {
        Specie? entity = await _species.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Specie with ID:{id} not found");
        }

         _species.Remove(entity);
        await SaveChangesAsync(null, cancellationToken);

        return id;
    }

    public async Task<IEnumerable<Specie>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _species.AsNoTracking()
                    .ToListAsync(cancellationToken);
    }

    public async Task<Specie?> GetByCriteriaAsync(Expression<Func<Specie, bool>> searchCriteria, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            return _species.Include(s => s.Breeds)
            .FirstOrDefault(searchCriteria);
        }, cancellationToken);
    }

    public Task<Specie?> GetByIdAsync(SpecieId id, CancellationToken cancellationToken)
    {
        return _species.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(Specie? entity, CancellationToken cancellationToken)
    {
        if (entity != null)
        {
             _species.Attach(entity);
        }
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
