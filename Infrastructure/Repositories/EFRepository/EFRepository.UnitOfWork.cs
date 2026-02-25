namespace Infrastructure.Repositories.EFRepository;

public partial class EFRepository<T>
{
    public Task BeginTransactionAsync(CancellationToken ct)
        => _db.Database.BeginTransactionAsync(ct);

    public Task CommitTransactionAsync(CancellationToken ct)
        => _db.Database.CommitTransactionAsync(ct);

    public Task RollbackTransactionAsync(CancellationToken ct)
        => _db.Database.RollbackTransactionAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}