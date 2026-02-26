using Application.Repositories;
using Application.Tasks.Interfaces;
using Domain.Entities;
using Application.Common.Exceptions;

namespace Application.Tasks.Services
{
    public class TaskService(ITaskRepository repo, IUnitOfWork unitOfWork) : ITaskService
    {
        private readonly ITaskRepository _repo = repo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public Task<List<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct)
            => _repo.GetAllAsync(userId, ct);

        public async Task<TaskItem> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var task = await _repo.GetByIdAsync(userId, id, ct) ?? throw new NotFoundException("Task not found", "TASK_NOT_FOUND");
            return task;
        }

        public async Task<TaskItem> CreateAsync(Guid userId, string title, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var task = TaskItem.Create(userId, title, now);

            await _repo.AddAsync(task, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return task;
        }

        public async Task MarkDoneAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var task = await _repo.GetByIdAsync(userId, id, ct) ?? throw new NotFoundException("Task not found", "TASK_NOT_FOUND");
            task.MarkDone();
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(userId, id, ct) ?? throw new NotFoundException("Task not found", "TASK_NOT_FOUND");
            await _repo.DeleteAsync(existing.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
