using Domain.Entities;
namespace Application.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct);
        Task DeleteAsync(Guid userId, Guid id, CancellationToken ct);
    }
}
