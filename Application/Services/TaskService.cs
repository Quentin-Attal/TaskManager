using Application.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repo;

        public TaskService(ITaskRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<TaskItem>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<TaskItem?> GetByIdAsync(Guid id)
            => _repo.GetByIdAsync(id);

        public async Task<TaskItem> CreateAsync(string title)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                IsDone = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repo.AddAsync(task);
            await _repo.SaveChangesAsync();
            return task;
        }

        public async Task<bool> MarkDoneAsync(Guid id)
        {
            var task = await _repo.GetByIdAsync(id);
            if (task is null) return false;

            task.IsDone = true;
            await _repo.UpdateAsync(task);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return false;

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
