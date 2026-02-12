using Application.Repositories;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TaskRepository(AppDbContext context) : ITaskRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync(ct);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
        {
            return await _context.Tasks.Where(t => t.UserId == userId && t.Id == id).FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(TaskItem task, CancellationToken ct)
        {
            await _context.Tasks.AddAsync(task, ct);
        }

        public Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var task = await GetByIdAsync(userId, id, ct);
            if (task != null)
                _context.Tasks.Remove(task);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}