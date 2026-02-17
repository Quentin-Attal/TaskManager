using Application.Auth.Interfaces;
using Application.Auth.Models;
using Application.Repositories;
using Contracts.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Application.Auth.Services
{
    public class AuthService(IUserRepository repo, IRefreshTokenRepository repoToken, ITokenService tokenService) : IAuthService
    {
        private readonly IUserRepository _repo = repo;
        private readonly IRefreshTokenRepository _repoToken = repoToken;
        private readonly ITokenService _tokenService = tokenService;

        private readonly PasswordHasher<AppUser> _hasher = new();

        public async Task<(AuthLoginResult? Result, AuthErrorCode Error)> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            string email = NormalizeEmail(request.Email);
            if (!IsEmailValid(email))
            {
                return (null, AuthErrorCode.InvalidEmail); ;
            }
            var user = await _repo.GetByEmailAsync(email, ct);
            if (user == null)
            {
                return (null, AuthErrorCode.InvalidCredentials); ;
            }
            if (_hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) != PasswordVerificationResult.Success)
            {
                return (null, AuthErrorCode.InvalidCredentials); ;
            }

            var lastToken = await _repoToken.GetActiveByUserId(user.Id);
            if (lastToken != null)
            {
                lastToken.RevokedAtUtc = DateTime.UtcNow;

                await _repoToken.SaveChangesAsync(ct);
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

            return (new AuthLoginResult(
                AccessToken: accessToken,
                RefreshTokenPlain: refresh.PlainToken,
                RefreshTokenExpiresAtUtc: refresh.ExpiresAtUtc
            ), AuthErrorCode.None);
        }

        public async Task<(AuthLoginResult? Result, AuthErrorCode Error)> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            string email = NormalizeEmail(request.Email);
            if (!IsEmailValid(email))
            {
                return (null, AuthErrorCode.InvalidEmail);
            }
            if (!IsPasswordStrong(request.Password))
            {
                return (null, AuthErrorCode.PasswordTooWeak);
            }
            if (request.Password != request.ConfirmPassword)
            {
                return (null, AuthErrorCode.InvalidCredentials);
            }
            var user = await _repo.GetByEmailAsync(email, ct);
            if (user != null)
            {
                return (null, AuthErrorCode.EmailAlreadyExists); ;
            }
            var appUser = new AppUser
            {
                CreatedAtUtc = DateTime.UtcNow,
                Email = email
            };
            appUser.PasswordHash = _hasher.HashPassword(appUser, request.Password);
            await _repo.AddAsync(appUser, ct);
            await _repo.SaveChangesAsync(ct);
            var loginRequest = new LoginRequest(request.Email, request.Password);
            return await LoginAsync(loginRequest, ct);
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

        public async Task<AuthRefreshResult?> RefreshAsync(string refreshTokenPlain, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
            {
                return null;
            }

            var hash = _tokenService.HashRefreshToken(refreshTokenPlain);

            var existing = await _repoToken.FindByHashAsync(hash, ct);
            if (existing is null)
                return null;

            if (!existing.IsActive)
            {
                await RevokeAllUserRefreshTokens(existing.UserId, ct);
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
                user = await _repo.GetByIdAsync(existing.UserId, ct);
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

        private async Task RevokeAllUserRefreshTokens(Guid userId, CancellationToken ct)
        {
            var lastToken = await _repoToken.GetActiveByUserId(userId);
            if (lastToken != null)
            {
                lastToken.RevokedAtUtc = DateTime.UtcNow;

                await _repoToken.SaveChangesAsync(ct);
            }
        }

        private static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();

        private static bool IsEmailValid(string email)
        {
            string pattern = @"^((?!\.)[\w\-_.+]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$";
            return Regex.IsMatch(email, pattern);
        }

        private static bool IsPasswordStrong(string password)
        {
            string pattern = @"^(?=(?:.*[A-Z]){1,})(?=(?:.*[a-z]){1,})(?=(?:.*\d){1,})(?=(?:.*[!@#$%^&*()\-_=+{};:,<.>]){1,})(?!.*(.)\1{2})([A-Za-z0-9!@#$%^&*()\-_=+{};:,<.>]{12,})";
            return Regex.IsMatch(password, pattern);
        }
    }
}
