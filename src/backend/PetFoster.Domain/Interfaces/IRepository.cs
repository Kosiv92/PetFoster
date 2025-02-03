using PetFoster.Domain.Entities;
using System.Linq.Expressions;

namespace PetFoster.Domain.Interfaces;

public interface IRepository<TEntity, TId> 
    where TEntity : SoftDeletableEntity<TId> 
    where TId : IComparable<TId>
{
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    public Task<TId> DeleteAsync(TId id, CancellationToken cancellationToken);

    public Task SaveChangesAsync(TEntity? entity, CancellationToken cancellationToken);

    public Task<TEntity?> GetByCriteriaAsync(Expression<Func<TEntity, bool>> searchCriteria, CancellationToken cancellationToken);
}
