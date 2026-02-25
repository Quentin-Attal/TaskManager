namespace Application.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<T?> GetByIdAsync(int id, CancellationToken ct);
        Task<T> AddAsync(T entity, CancellationToken ct);
        void Update(T entity);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}