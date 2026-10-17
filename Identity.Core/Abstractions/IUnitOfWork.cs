namespace Identity.Core.Abstractions;

public interface IUnitOfWork
{
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    Task BeginTransaction(CancellationToken ct = default);
    Task CommitTransaction(CancellationToken ct = default);
    Task RollbackTransaction(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}