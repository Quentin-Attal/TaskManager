using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(AppUser user, CancellationToken ct);
        Task UpdateAsync(AppUser user);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
