using Domain.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.EFRepository;

public partial class EFRepository<T>
{
    public Task<List<T>> GetAllAsync(CancellationToken ct)
        => _db.Set<T>().AsNoTracking().ToListAsync(ct);

    public Task<List<T>> ListAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct)
        => ApplySpecification(spec, options).ToListAsync(ct);

    public Task<int> CountAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct)
        => ApplySpecification(spec, options).CountAsync(ct);

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct)
        => ApplySpecification(spec, options).FirstOrDefaultAsync(ct);

    public Task<T?> SingleOrDefaultAsync(ISpecification<T> spec, QueryOptions options, CancellationToken ct)
        => ApplySpecification(spec, options).SingleOrDefaultAsync(ct);


    public async Task<T?> GetByIdAsync(int id, CancellationToken ct) => await _db.Set<T>().FindAsync([id], cancellationToken: ct);
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct) => await _db.Set<T>().FindAsync([id], cancellationToken: ct);
}