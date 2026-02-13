using Application.Auth.Models;
using Contracts.Auth;

namespace Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthLoginResult?> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<AuthRefreshResult?> RefreshAsync(Guid userId, string refreshTokenPlain, CancellationToken ct);
        Task LogoutAsync(Guid userId, string? refreshTokenPlain, CancellationToken ct);
    }

}
