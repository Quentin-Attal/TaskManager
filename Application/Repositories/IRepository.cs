namespace Application.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(CancellationToken ct);
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task CreateTransactionAsync(CancellationToken ct);
        Task CommitTransactionAsync(CancellationToken ct);
    }
}