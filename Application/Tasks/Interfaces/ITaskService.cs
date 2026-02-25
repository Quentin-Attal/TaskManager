using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Tasks.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct);
        Task<TaskItem> CreateAsync(Guid userId, string title, CancellationToken ct);
        Task<bool> MarkDoneAsync(Guid userId, Guid id, CancellationToken ct);
        Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct);
    }
}
