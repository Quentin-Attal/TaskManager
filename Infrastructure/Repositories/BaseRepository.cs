using Application.Repositories;

namespace Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IEFCRUDRepository<T> _crud;

        protected BaseRepository(IEFCRUDRepository<T> crud)
        {
            _crud = crud;
        }

        public Task AddAsync(T entity) => _crud.AddAsync(entity);
        public Task DeleteAsync(Guid id, CancellationToken ct) => _crud.DeleteAsync(id, ct);
        public Task DeleteAsync(int id, CancellationToken ct) => _crud.DeleteAsync(id, ct);
        public Task<List<T>> GetAllAsync(CancellationToken ct) => _crud.GetAllAsync(ct);
        public Task<List<T>> GetAllReadOnlyAsync(CancellationToken ct) => _crud.GetAllAsync(ct);
        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct) => _crud.GetByIdAsync(id, ct);
        public Task<T?> GetByIdReadOnlyAsync(Guid id, CancellationToken ct) => _crud.GetByIdAsync(id, ct);
        public Task<T?> GetByIdAsync(int id, CancellationToken ct) => _crud.GetByIdAsync(id, ct);
        public Task<T?> GetByIdReadOnlyAsync(int id, CancellationToken ct) => _crud.GetByIdAsync(id, ct);
        public Task SaveChangesAsync(CancellationToken ct) => _crud.SaveChangesAsync(ct);
        public Task UpdateAsync(T entity) => _crud.UpdateAsync(entity);
        public Task CreateTransactionAsync(CancellationToken ct) => _crud.CreateTransactionAsync(ct);
        public Task CommitTransactionAsync(CancellationToken ct) => _crud.CommitTransactionAsync(ct);
        public Task RollbackTransactionAsync(CancellationToken ct) => _crud.RollbackTransactionAsync(ct);
    }
}