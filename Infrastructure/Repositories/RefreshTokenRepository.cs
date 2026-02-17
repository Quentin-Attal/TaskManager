using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
    {
        private readonly AppDbContext _db = db;

        public Task<RefreshToken?> FindByHashAsync(Guid userId, string tokenHash, CancellationToken ct)
        {
            return _db.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash && rt.UserId == userId, ct);
        }

        public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct)
        {
            return _db.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            await _db.RefreshTokens.AddAsync(token, ct);
        }
        public Task<RefreshToken?> GetActiveByUserId(Guid id, CancellationToken ct)
        {
            return _db.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.UserId == id && rt.RevokedAtUtc == null && rt.ExpiresAtUtc > DateTime.UtcNow, ct);
        }

        public Task<List<RefreshToken>> GetActivesByUserId(Guid id, CancellationToken ct)
        {
            return _db.RefreshTokens
                .Where(rt => rt.UserId == id && rt.RevokedAtUtc == null && rt.ExpiresAtUtc > DateTime.UtcNow)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }
    }
}
