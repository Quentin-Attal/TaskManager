using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct);
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<RefreshToken?> GetByUserId(Guid id);
    }
}
