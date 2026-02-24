using Domain.Entities;
namespace Application.Repositories
{
    public interface IRefreshTokenRepository: IRepository<RefreshToken>
    {
        Task<RefreshToken?> FindByHashAsync(Guid id, string tokenHash, CancellationToken ct);
        Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct);
        Task<RefreshToken?> GetActiveByUserId(Guid id, CancellationToken ct);
        Task<List<RefreshToken>> GetActivesByUserId(Guid id, CancellationToken ct);
    }
}
