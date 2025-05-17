using Microsoft.EntityFrameworkCore;
using PetFoster.Core.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Infrastructure.DbContexts;
using System.Linq.Expressions;

namespace PetFoster.Infrastructure.Repositories
{
    public sealed class VolunteersRepository : IRepository<Volunteer, VolunteerId>
    {
        private readonly WriteDbContext _dbContext;
        private readonly DbSet<Volunteer> _volunteers;

        public VolunteersRepository(WriteDbContext dbContext)
        {
            _dbContext = dbContext;
            _volunteers = _dbContext.Volunteers;
        }

        public async Task AddAsync(Volunteer entity, CancellationToken cancellationToken = default)
        {
            _ = await _volunteers.AddAsync(entity, cancellationToken);
            await SaveChangesAsync(null, cancellationToken);
        }

        public async Task<VolunteerId> DeleteAsync(VolunteerId id, CancellationToken cancellationToken = default)
        {
            Volunteer? entity = await _volunteers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Volunteer with ID:{id} not found");
            }

            _ = _volunteers.Remove(entity);
            await SaveChangesAsync(null, cancellationToken);

            return id;
        }

        public async Task<IEnumerable<Volunteer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _volunteers
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public Task<Volunteer?> GetByIdAsync(VolunteerId id, CancellationToken cancellationToken = default)
        {
            return _volunteers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task SaveChangesAsync(Volunteer? entity, CancellationToken cancellationToken = default)
        {
            if (entity != null)
            {
                _ = _volunteers.Attach(entity);
            }
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Volunteer?> GetByCriteriaAsync(Expression<Func<Volunteer, bool>> searchCriteria, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return _volunteers.FirstOrDefault(searchCriteria);
            }, cancellationToken);
        }
    }
}