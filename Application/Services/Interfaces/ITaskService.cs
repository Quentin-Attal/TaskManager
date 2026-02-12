using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> CreateAsync(string title);
        Task<bool> MarkDoneAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
