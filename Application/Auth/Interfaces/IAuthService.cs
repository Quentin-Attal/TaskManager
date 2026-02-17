using Application.Auth.Models;
using Contracts.Auth;

namespace Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthLoginResult? Result, AuthErrorCode Error)> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<(AuthLoginResult? Result, AuthErrorCode Error)> RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task<AuthRefreshResult?> RefreshAsync(string refreshTokenPlain, CancellationToken ct);
        Task LogoutAsync(Guid userId, CancellationToken ct);
    }

}
