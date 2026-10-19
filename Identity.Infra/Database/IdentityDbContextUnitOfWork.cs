using System.Text.Json;
using Base;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infra.Database;

public partial class IdentityDbContext : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private List<IDomainEvent> _domainEvents = new();

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

    public void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    void IUnitOfWork.Add<TEntity>(TEntity entity) => Add(entity);
    void IUnitOfWork.Remove<TEntity>(TEntity entity) => Remove(entity);

    public async Task<int> SaveChanges(CancellationToken ct = default)
    {
        if (_domainEvents.Any())
        {
            var events = _domainEvents.Select(e => new Outbox
            {
                EventType = e.GetType().FullName ?? string.Empty,
                Payload = JsonSerializer.Serialize(e),
                CreatedAt = DateTimeOffset.UtcNow
            })
            .ToList();
            
            Outbox.AddRange(events);
        }
        
        var result = await base.SaveChangesAsync(ct);
        _domainEvents.Clear();

        return result;
    }
}