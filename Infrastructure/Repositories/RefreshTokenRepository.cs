using Application.Repositories;
using Domain.Entities;
using Domain.Specification.Refresh;

namespace Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository: Repository<RefreshToken>, IRefreshTokenRepository
    {   
        public RefreshTokenRepository(ICRUDRepository<RefreshToken> repository) : base(repository)
        {
        }
        
        public Task<RefreshToken?> FindByHashAsync(Guid userId, string tokenHash, CancellationToken ct)
        {
            return _crud.SingleOrDefaultAsync(new RefreshTokenByTokenHashAndUserIdSpecification(userId, tokenHash), ct);
        }

        public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct)
        {
            return _crud.SingleOrDefaultAsync(new RefreshTokenByTokenHashSpecification(tokenHash), ct);
        }
        public Task<RefreshToken?> GetActiveByUserId(Guid id, CancellationToken ct)
        {
            return _crud.SingleOrDefaultAsync(new RefreshTokenByActiveUserSpecification(id), ct);
        }

        public Task<List<RefreshToken>> GetActivesByUserId(Guid id, CancellationToken ct)
        {
            return _crud.ListAsync(new RefreshTokenByActiveUserSpecification(id), ct);
        }
    }
}
