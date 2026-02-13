using Application.Auth.Interfaces;
using Application.Auth.Models;
using Application.Repositories;
using Contracts.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Services
{
    public class AuthService(IUserRepository repo, IRefreshTokenRepository repoToken, ITokenService tokenService) : IAuthService
    {
        private readonly IUserRepository _repo = repo;
        private readonly IRefreshTokenRepository _repoToken = repoToken;
        private readonly ITokenService _tokenService = tokenService;

        private readonly PasswordHasher<AppUser> _hasher = new();

        public async Task<AuthLoginResult?> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            string email = NormalizeEmail(request.Email);
            var user = await _repo.GetByEmailAsync(email, ct);
            if (user == null)
            {
                return null;
            }
            if (_hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) != PasswordVerificationResult.Success)
            {
                return null;
            }
            var accessToken = _tokenService.CreateAccessToken(user);

            var refresh = _tokenService.CreateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refresh.TokenHash,
                ExpiresAtUtc = refresh.ExpiresAtUtc
            };

            await _repoToken.AddAsync(refreshEntity, ct);
            await _repoToken.SaveChangesAsync(ct);

            return new AuthLoginResult(
                AccessToken: accessToken,
                RefreshTokenPlain: refresh.PlainToken,
                RefreshTokenExpiresAtUtc: refresh.ExpiresAtUtc
            );
        }

        public async Task LogoutAsync(Guid userId, string? refreshTokenPlain, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
                return;

            var hash = _tokenService.HashRefreshToken(refreshTokenPlain);

            var token = await _repoToken.FindByHashAsync(userId, hash, ct);
            if (token is null || token.RevokedAtUtc is not null)
                return;

            token.RevokedAtUtc = DateTime.UtcNow;

            await _repoToken.SaveChangesAsync(ct);
        }

        public async Task<AuthRefreshResult?> RefreshAsync(Guid userId, string refreshTokenPlain, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
            {
                return null;
            }

            var hash = _tokenService.HashRefreshToken(refreshTokenPlain);

            var existing = await _repoToken.FindByHashAsync(userId, hash, ct);
            if (existing is null)
                return null;

            if (!existing.IsActive)
            {
                // TOKEN REUSE DETECTED
                await RevokeAllUserRefreshTokens(userId);
                return null;
            }

            existing.RevokedAtUtc = DateTime.UtcNow;


            var refresh = _tokenService.CreateRefreshToken();

            existing.ReplacedByTokenHash = refresh.TokenHash;

            await _repoToken.AddAsync(new RefreshToken
            {
                UserId = existing.UserId,
                TokenHash = refresh.TokenHash,
                ExpiresAtUtc = refresh.ExpiresAtUtc
            }, ct);

            var user = existing.User;
            if (user is null)
            {
                user = await _repo.GetByIdAsync(userId, ct);
                if (user is null) return null;
            }

            var accessToken = _tokenService.CreateAccessToken(user);

            await _repoToken.SaveChangesAsync(ct);

            return new AuthRefreshResult(
                AccessToken: accessToken,
                RefreshTokenPlain: refresh.PlainToken,
                RefreshTokenExpiresAtUtc: refresh.ExpiresAtUtc
            );
        }

        private static async Task RevokeAllUserRefreshTokens(Guid userId)
        {
            throw new NotImplementedException();
        }

        private static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
    }
}
