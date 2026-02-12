using Application.Auth.Models;
using Contracts.Auth;

namespace Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthLoginResult?> LoginAsync(LoginRequest request, AuthRequestContext context, CancellationToken ct);
        Task<AuthRefreshResult?> RefreshAsync(string refreshTokenPlain, AuthRequestContext context, CancellationToken ct);
        Task LogoutAsync(string? refreshTokenPlain, AuthRequestContext context, CancellationToken ct);
    }

}
