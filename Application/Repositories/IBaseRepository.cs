namespace Application.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(CancellationToken ct);
        Task<List<T>> GetAllReadOnlyAsync(CancellationToken ct);
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<T?> GetByIdAsync(int id, CancellationToken ct);
        Task<T?> GetByIdReadOnlyAsync(int id, CancellationToken ct);
        Task<T?> GetByIdReadOnlyAsync(Guid id, CancellationToken ct);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task CreateTransactionAsync(CancellationToken ct);
        Task CommitTransactionAsync(CancellationToken ct);
        Task RollbackTransactionAsync(CancellationToken ct);
    }
}