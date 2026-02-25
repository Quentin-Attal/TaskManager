using Application.Repositories;
using Domain.Entities;
using Domain.Specification.Refresh;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Repositories.EFRepository;

namespace Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository(IEFRepository<RefreshToken> repository) : BaseRepository<RefreshToken>(repository), IRefreshTokenRepository
    {
        public Task<RefreshToken?> FindByHashAsync(Guid userId, string tokenHash, CancellationToken ct)
        {
            return _crud.SingleOrDefaultAsync(new RefreshTokenByTokenHashAndUserIdSpecification(userId, tokenHash), QueryOptions.Default, ct);
        }

        public Task<RefreshToken?> FindByHashWithUserAsync(string tokenHash, CancellationToken ct)
        {
            return _crud.SingleOrDefaultAsync(new RefreshTokenByTokenHashSpecification(tokenHash), QueryOptions.Default, ct);
        }
        public Task<RefreshToken?> GetActiveByUserId(Guid id, CancellationToken ct)
        {
            return _crud.SingleOrDefaultAsync(new RefreshTokenByActiveUserSpecification(id), QueryOptions.Default, ct);
        }

        public Task<List<RefreshToken>> GetActivesByUserId(Guid id, CancellationToken ct)
        {
            return _crud.ListAsync(new RefreshTokenByActiveUserSpecification(id), QueryOptions.Default, ct);
        }
    }
}
