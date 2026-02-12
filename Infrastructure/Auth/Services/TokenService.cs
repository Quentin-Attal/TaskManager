using Application.Auth.Interfaces;
using Application.Auth.Models;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwt;
        private readonly TokenHashOptions _hash;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public TokenService(
            IOptions<JwtOptions> jwtOptions,
            IOptions<TokenHashOptions> hashOptions,
            IRefreshTokenRepository refreshTokenRepo)
        {
            _jwt = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _hash = hashOptions?.Value ?? throw new ArgumentNullException(nameof(hashOptions));
            _refreshTokenRepo = refreshTokenRepo ?? throw new ArgumentNullException(nameof(refreshTokenRepo));

            if (string.IsNullOrWhiteSpace(_jwt.Key))
                throw new InvalidOperationException("Jwt:SigningKey is required.");

            if (string.IsNullOrWhiteSpace(_jwt.Issuer))
                throw new InvalidOperationException("Jwt:Issuer is required.");

            if (string.IsNullOrWhiteSpace(_jwt.Audience))
                throw new InvalidOperationException("Jwt:Audience is required.");
        }

        public string CreateAccessToken(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);
            if (user.Id == Guid.Empty) throw new ArgumentException("User.Id is required.", nameof(user));

            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(JwtRegisteredClaimNames.Iat, ToUnixTimeSeconds(now).ToString(), ClaimValueTypes.Integer64),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_jwt.AccessTokenMinutes <= 0 ? 15 : _jwt.AccessTokenMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public RefreshTokenDescriptor CreateRefreshToken()
        {
            var now = DateTime.UtcNow;

            var plain = TokenUtils.NewRefreshToken();

            return new RefreshTokenDescriptor
            (
                plain,
                HashRefreshToken(plain),
                now.AddDays(_jwt.RefreshTokenDays <= 0 ? 30 : _jwt.RefreshTokenDays)
            );
        }

        public string GetAllRefreshTokenByUserId(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
            return String.Empty;
        }

        public string HashRefreshToken(string refreshTokenPlain)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
                throw new ArgumentException("Refresh token is required.", nameof(refreshTokenPlain));

            // Pepper strongly recommended. If you truly don't want it, pass empty and TokenUtils can still hash.
            var pepper = _hash?.Pepper ?? string.Empty;
            return TokenUtils.HashRefreshToken(refreshTokenPlain, pepper);
        }

        private static long ToUnixTimeSeconds(DateTime utc)
        {
            if (utc.Kind != DateTimeKind.Utc)
                utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(utc - epoch).TotalSeconds;
        }
    }
}
