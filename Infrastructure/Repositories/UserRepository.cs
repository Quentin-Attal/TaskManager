using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(AppUser user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Users.FindAsync(new object?[] { id, ct }, cancellationToken: ct);
        }
        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var user = await GetByIdAsync(id, ct);
            if (user != null)
                _context.Users.Remove(user);
        }

        public Task UpdateAsync(AppUser user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
