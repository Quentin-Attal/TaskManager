using Domain.Entities;
namespace Application.Repositories
{
    public interface ITaskRepository : IBaseRepository<TaskItem>
    {
        Task<List<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct);
    }
}
