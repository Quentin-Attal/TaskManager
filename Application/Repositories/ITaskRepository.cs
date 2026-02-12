using Domain.Entities;
namespace Application.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct);
        Task AddAsync(TaskItem task, CancellationToken ct);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(Guid userId, Guid id, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
