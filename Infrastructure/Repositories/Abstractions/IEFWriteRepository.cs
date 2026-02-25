using Domain.Specification;

namespace Infrastructure.Repositories.Abstractions;

public interface IEFWriteRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken ct);
    Task<List<T>> AddAsync(List<T> entities, CancellationToken ct);

    void UpdateAsync(T entity);
    void UpdateAsync(List<T> entities);

    Task DeleteAsync(Guid id, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    void DeleteAsync(T entity);
    void DeleteAsync(IEnumerable<T> entities);
    Task DeleteAsync(ISpecification<T> spec, CancellationToken ct);
}