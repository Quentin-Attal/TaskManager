using System.Linq.Expressions;
using Application.Repositories;
using Domain.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Repositories
{
    public class CRUDRepository<T>(AppDbContext db) : ICRUDRepository<T>, IAsyncDisposable where T : class
    {
        protected readonly AppDbContext _db = db;

        public Task<T> AddAsync(T entity)
        {
            _db.Set<T>().Add(entity);
            return Task.FromResult(entity);
        }

        public async Task<List<T>> AddAsync(List<T> entity, CancellationToken ct)
        {
            await _db.Set<T>().AddRangeAsync(entity, ct);
            return entity;
        }


        public Task CreateTransactionAsync(CancellationToken ct) => _db.Database.BeginTransactionAsync(ct);

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _db.Set<T>().FindAsync([id, ct], cancellationToken: ct);
            if (entity != null)
            {
                _db.Set<T>().Remove(entity);
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var entity = await _db.Set<T>().FindAsync([id, ct], cancellationToken: ct);
            if (entity != null)
            {
                _db.Set<T>().Remove(entity);
            }
        }


        public Task DeleteAsync(T entity)
        {
            _db.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }


        public async Task DeleteAsync(ISpecification<T> spec, CancellationToken ct)
        {
            if (spec == null || spec.Criteria == null) throw new ArgumentException("Specification must have a criteria to delete entities");
            Expression<Func<T, bool>> predicate = spec.Criteria;

            await _db.Set<T>()
                .Where(predicate)
                .ExecuteDeleteAsync(ct);
        }

        public Task<List<T>> GetAllAsync(CancellationToken ct) => _db.Set<T>().ToListAsync(ct);

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct) => await _db.Set<T>().FindAsync([id, ct], cancellationToken: ct);
        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct) => await _db.Set<T>().FindAsync([id, ct], cancellationToken: ct);

        public Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken ct) => ApplySpecification(spec).ToListAsync(ct);

        public Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct) => ApplySpecification(spec).CountAsync(ct);
        public Task<T?> SingleOrDefaultAsync(ISpecification<T> spec, CancellationToken ct) => ApplySpecification(spec).SingleOrDefaultAsync(ct);

        public Task UpdateAsync(T entity)
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

            return Task.CompletedTask;
        }

        public async Task UpdateAsync(List<T> entities)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity);
            }
        }
        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct) => await ApplySpecification(spec).FirstOrDefaultAsync(ct);

        public async Task<T> FirstAsync(ISpecification<T> spec, CancellationToken ct) => await ApplySpecification(spec).FirstAsync(ct);

        public async Task<T?> LastOrDefaultAsync(ISpecification<T> spec, CancellationToken ct) => await ApplySpecification(spec).LastOrDefaultAsync(ct);


        private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
           SpecificationEvaluator<T>.GetQuery(_db.Set<T>().AsQueryable(), spec);



        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class => _db.Set<TEntity>();

        public IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class =>
                    _db.Set<TEntity>().Where(predicate);

        public IQueryable<TEntity> GetQuery<TEntity>(ISpecification<TEntity> criteria) where TEntity : class =>
             SpecificationEvaluator<TEntity>.GetQuery(_db.Set<TEntity>().AsQueryable(), criteria);

        public Task BeginTransactionAsync(CancellationToken cancellationToken) => _db.Database.BeginTransactionAsync(cancellationToken);
        public Task CommitTransactionAsync(CancellationToken cancellationToken) => _db.Database.CommitTransactionAsync(cancellationToken);
        public Task RollbackTransactionAsync(CancellationToken cancellationToken) => _db.Database.RollbackTransactionAsync(cancellationToken);
        public Task SaveChangesAsync(CancellationToken cancellationToken) => _db.SaveChangesAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            await _db.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}