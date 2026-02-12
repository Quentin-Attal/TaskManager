using Application.Repositories;
using Application.Tasks.Interfaces;
using Domain.Entities;

namespace Application.Tasks.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repo;

        public TaskService(ITaskRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct)
            => _repo.GetAllAsync(userId, ct);

        public Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
            => _repo.GetByIdAsync(userId, id, ct);

        public async Task<TaskItem> CreateAsync(Guid userId, string title, CancellationToken ct)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                IsDone = false,
                CreatedAtUtc = DateTime.UtcNow,
                UserId = userId,
            };

            await _repo.AddAsync(task, ct);
            await _repo.SaveChangesAsync(ct);
            return task;
        }

        public async Task<bool> MarkDoneAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var task = await _repo.GetByIdAsync(userId, id, ct);
            if (task is null) return false;

            task.IsDone = true;
            await _repo.UpdateAsync(task);
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(userId, id, ct);
            if (existing is null) return false;

            await _repo.DeleteAsync(userId, id, ct);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
