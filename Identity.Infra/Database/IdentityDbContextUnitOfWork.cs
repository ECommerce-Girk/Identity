using Identity.Core.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infra.Database;

public partial class IdentityDbContext : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransaction(CancellationToken ct = default)
    {
        if (_transaction != null) return;

        _transaction ??= await Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransaction(CancellationToken ct = default)
    {
        if (_transaction is null) return;

        try
        {
            await _transaction.CommitAsync(ct);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransaction(CancellationToken ct = default)
    {
        if (_transaction is null) return;

        try
        {
            await _transaction.RollbackAsync(ct);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    void IUnitOfWork.Add<TEntity>(TEntity entity) => Add(entity);
    void IUnitOfWork.Remove<TEntity>(TEntity entity) => Remove(entity);
}