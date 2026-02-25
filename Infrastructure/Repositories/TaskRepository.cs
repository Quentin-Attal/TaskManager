using Application.Repositories;
using Domain.Entities;
using Domain.Specification.Task;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Repositories.EFRepository;

namespace Infrastructure.Repositories
{
    public class TaskRepository(IEFRepository<TaskItem> repository) : BaseRepository<TaskItem>(repository), ITaskRepository
    {
        public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct)
        {
            return await _crud.ListAsync(new TaskByUserSpecification(userId), QueryOptions.ReadOnly, ct);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
        {
            return await _crud.SingleOrDefaultAsync(new TaskByUserAndIdSpecification(userId, id), QueryOptions.Default, ct);
        }


        public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var task = await GetByIdAsync(userId, id, ct);
            if (task != null)
                _crud.DeleteAsync(task);
        }

    }
}