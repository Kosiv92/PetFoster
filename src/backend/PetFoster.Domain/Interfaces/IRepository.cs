using CSharpFunctionalExtensions;

namespace PetFoster.Domain.Interfaces;

public interface IRepository<TEntity, TId> where TEntity : Entity<TId> where TId : IComparable<TId>
{
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    public Task DeleteAsync(TId id, CancellationToken cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken);    
}
