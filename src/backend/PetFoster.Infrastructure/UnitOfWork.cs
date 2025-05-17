using Microsoft.EntityFrameworkCore.Storage;
using PetFoster.Core.Database;
using PetFoster.Infrastructure.DbContexts;
using System.Data;

namespace PetFoster.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WriteDbContext _dbContext;

        public UnitOfWork(WriteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
        {
            IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            return transaction.GetDbTransaction();
        }

        public async Task SaveChanges(CancellationToken cancellationToken = default)
        {
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
