using Application.Repositories;
using Domain.Entities;
using Domain.Specification.Task;

namespace Infrastructure.Repositories
{
    public class TaskRepository(ICRUDRepository<TaskItem> repository) : Repository<TaskItem>(repository), ITaskRepository
    {
        public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct)
        {
            return await _crud.ListAsync(new TaskByUserSpecification(userId), ct);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
        {
            return await _crud.SingleOrDefaultAsync(new TaskByUserAndIdSpecification(userId, id), ct);
        }


        public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var task = await GetByIdAsync(userId, id, ct);
            if (task != null)
                await _crud.DeleteAsync(task);
        }

    }
}