namespace Infrastructure.Repositories.Abstractions;

public interface IEFUnitOfWorkRepository
{
    Task BeginTransactionAsync(CancellationToken ct);
    Task CommitTransactionAsync(CancellationToken ct);
    Task RollbackTransactionAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}