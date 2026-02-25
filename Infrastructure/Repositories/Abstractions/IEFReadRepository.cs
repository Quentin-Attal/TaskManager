using Domain.Specification;
using Infrastructure.Repositories.EFRepository;

namespace Infrastructure.Repositories.Abstractions;

public interface IEFReadRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(CancellationToken ct);

    Task<List<T>> ListAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct);

    Task<int> CountAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct);

    Task<T?> SingleOrDefaultAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct);

    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<T?> GetByIdAsync(int id, CancellationToken ct);
}