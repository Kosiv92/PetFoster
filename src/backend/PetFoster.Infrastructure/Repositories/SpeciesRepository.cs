using Microsoft.EntityFrameworkCore;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using System.Linq.Expressions;

namespace PetFoster.Infrastructure.Repositories
{
    public sealed class SpeciesRepository : IRepository<Specie, SpecieId>
    {
        private readonly ApplicationDbContext _dbContext;
        DbSet<Specie> _species;

        public SpeciesRepository(ApplicationDbContext dbContext)
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
            var entity = await _species.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
                throw new Exception($"Volunteer with ID:{id} not found");

            _species.Remove(entity);
            await SaveChangesAsync(null, cancellationToken);

            return id;
        }

        public async Task<IEnumerable<Specie>> GetAllAsync(CancellationToken cancellationToken)
            => await _species.AsNoTracking()
                .ToListAsync(cancellationToken);
        
        public async Task<Specie?> GetByCriteriaAsync(Expression<Func<Specie, bool>> searchCriteria, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return _species.Include(s => s.Breeds)
                .FirstOrDefault(searchCriteria);
            }, cancellationToken);
        }

        public Task<Specie?> GetByIdAsync(SpecieId id, CancellationToken cancellationToken)
            => _species.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public Task SaveChangesAsync(Specie? entity, CancellationToken cancellationToken)
        {
            if (entity != null)
            {
                _species.Attach(entity);
            }
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
