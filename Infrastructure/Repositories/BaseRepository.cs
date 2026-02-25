using Application.Repositories;
using Infrastructure.Repositories.Abstractions;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository<T>(IEFRepository<T> crud) : IBaseRepository<T> where T : class
    {
        protected readonly IEFRepository<T> _crud = crud;

        public Task<T> AddAsync(T entity, CancellationToken ct) => _crud.AddAsync(entity, ct);
        public void UpdateAsync(T entity) => _crud.UpdateAsync(entity);
        public Task DeleteAsync(Guid id, CancellationToken ct) => _crud.DeleteAsync(id, ct);
        public Task DeleteAsync(int id, CancellationToken ct) => _crud.DeleteAsync(id, ct);
        public Task<List<T>> GetAllAsync(CancellationToken ct) => _crud.GetAllAsync(ct);
        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct) => _crud.GetByIdAsync(id, ct);
        public Task<T?> GetByIdAsync(int id, CancellationToken ct) => _crud.GetByIdAsync(id, ct);
        public Task SaveChangesAsync(CancellationToken ct) => _crud.SaveChangesAsync(ct);
        public Task BeginTransactionAsync(CancellationToken ct) => _crud.BeginTransactionAsync(ct);
        public Task CommitTransactionAsync(CancellationToken ct) => _crud.CommitTransactionAsync(ct);
        public Task RollbackTransactionAsync(CancellationToken ct) => _crud.RollbackTransactionAsync(ct);
    }
}