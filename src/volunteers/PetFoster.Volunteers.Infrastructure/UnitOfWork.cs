using Microsoft.EntityFrameworkCore.Storage;
using PetFoster.Core.Abstractions;
using PetFoster.Volunteers.Infrastructure.DbContexts;
using System.Data;

namespace PetFoster.Volunteers.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _dbContext;

    public UnitOfWork(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(
        CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction
            = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public Task SaveChanges(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
