using Infrastructure.Auth.Options;
using Infrastructure.Auth.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UnitTests.Helpers
{
    public static class TokenServiceTestHelpers
    {
        public static TokenService CreateSut(
        JwtOptions? jwt = null,
        TokenHashOptions? hash = null)
        {
            jwt ??= new JwtOptions
            {
                Key = "THIS_IS_A_TEST_SIGNING_KEY_32+CHARS_LONG!!",
                Issuer = "test-issuer",
                Audience = "test-audience",
                AccessTokenMinutes = 5,
                RefreshTokenDays = 7
            };

            hash ??= new TokenHashOptions
            {
                Pepper = "test-pepper"
            };

            return new TokenService(Options.Create(jwt), Options.Create(hash));
        }

        public static ClaimsPrincipal ValidateJwt(string token, JwtOptions jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(1) // reduce flakiness
            };

            return handler.ValidateToken(token, parameters, out _);
        }
    }
}
