using Domain.Specification;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public interface IEFCRUDRepository<T> : IAsyncDisposable where T : class
    {
        // Create
        Task<T> AddAsync(T entity);
        Task<List<T>> AddAsync(List<T> entities, CancellationToken ct);

        // Read
        Task<List<T>> GetAllAsync(CancellationToken ct);
        Task<T?> GetByIdAsync(int id, CancellationToken ct);
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken ct);
        Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct);
        Task<T> FirstAsync(ISpecification<T> spec, CancellationToken ct);
        Task<T?> LastOrDefaultAsync(ISpecification<T> spec, CancellationToken ct);
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct);
        Task<T?> SingleOrDefaultAsync(ISpecification<T> spec, CancellationToken ct);

        // Update
        Task UpdateAsync(T entity);
        Task UpdateAsync(List<T> entities);

        // Delete
        Task DeleteAsync(int id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(T entity);
        Task DeleteAsync(IEnumerable<T> entities);
        Task DeleteAsync(ISpecification<T> spec, CancellationToken ct);

        // Transactions
        Task CreateTransactionAsync(CancellationToken ct);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);


        // Save changes
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}