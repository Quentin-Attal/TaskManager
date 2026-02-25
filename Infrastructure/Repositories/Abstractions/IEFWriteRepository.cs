using Domain.Specification;

namespace Infrastructure.Repositories.Abstractions;

public interface IEFWriteRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken ct);
    Task<List<T>> AddAsync(List<T> entities, CancellationToken ct);

    void Update(T entity);
    void Update(List<T> entities);

    Task DeleteAsync(Guid id, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    void Delete(T entity);
    void Delete(IEnumerable<T> entities);
    Task DeleteAsync(ISpecification<T> spec, CancellationToken ct);
}