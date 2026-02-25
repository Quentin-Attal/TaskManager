using Domain.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Repositories.EFRepository;

public partial class EFRepository<T>
{
    public async Task<T> AddAsync(T entity, CancellationToken ct)
    {
        await _db.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public async Task<List<T>> AddAsync(List<T> entities, CancellationToken ct)
    {
        await _db.Set<T>().AddRangeAsync(entities, ct);
        return entities;
    }

    public void Delete(T entity)
    {
        _db.Set<T>().Remove(entity);
    }

    public void Delete(IEnumerable<T> entities)
    {
        _db.Set<T>().RemoveRange(entities);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.Set<T>().FindAsync([id], cancellationToken: ct);
        if (entity is not null) _db.Set<T>().Remove(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Set<T>().FindAsync([id], cancellationToken: ct);
        if (entity is not null) _db.Set<T>().Remove(entity);
    }

    public async Task DeleteAsync(ISpecification<T> spec, CancellationToken ct)
    {
        if (spec?.Criteria is null)
            throw new ArgumentException("Specification must have a criteria to delete entities");

        await _db.Set<T>().Where(spec.Criteria).ExecuteDeleteAsync(ct);
    }

    public void Update(T entity)
    {
        IEntityType? entityType = _db.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");
        IKey? primaryKey = entityType.FindPrimaryKey() ?? throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have a primary key.");
        object?[] keyValues = [.. primaryKey.Properties.Select(p => p.PropertyInfo?.GetValue(entity))];
        if (keyValues is null || keyValues.Length == 0)
            throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have a valid primary key value.");

        // Try to find a tracked entity with the same key
        T? trackedEntity = _db.Set<T>().Local
            .FirstOrDefault(e => primaryKey.Properties
                .Select(p => p.PropertyInfo?.GetValue(e))
                .SequenceEqual(keyValues));

        if (trackedEntity is not null)
        {
            // Already tracked: update its values
            _db.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            // Not tracked: attach and mark as modified
            _db.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        _db.Entry(entity).State = EntityState.Modified;
    }

    public void Update(List<T> entities)
    {
        foreach (var e in entities)
            Update(e);
    }
}