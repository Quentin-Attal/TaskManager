using Contracts.Auth;
using Domain.Entities;
using Infrastructure.Auth.Options;
using Infrastructure.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static UnitTests.Helpers.TokenServiceTestHelpers;

namespace UnitTests
{
    public class TokenServiceTest
    {
        [Fact]
        public async Task Should_CreateAccessToken()
        {
            var option = new JwtOptions
            {
                Key = "THIS_IS_A_TEST_SIGNING_KEY_32+CHARS_LONG!!",
                Issuer = "test-issuer",
                Audience = "test-audience",
                AccessTokenMinutes = 5,
                RefreshTokenDays = 7
            };
            string email = "email@mail.com";

            var user = new AppUser
            {
                Email = email,
                Id = Guid.NewGuid(),
            };

            var sut = CreateSut(option);
            var token = sut.CreateAccessToken(user);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            Assert.Equal(option.Issuer, jwt.Issuer);
            Assert.Contains(option.Audience, jwt.Audiences);

            var principal = ValidateJwt(token, option);

            Assert.Equal(user.Id.ToString(), principal.FindFirstValue(ClaimTypes.NameIdentifier));

            Assert.False(string.IsNullOrWhiteSpace(principal.FindFirstValue(JwtRegisteredClaimNames.Jti)));
            Assert.False(string.IsNullOrWhiteSpace(principal.FindFirstValue(JwtRegisteredClaimNames.Iat)));
        }

        [Fact]
        public void CreateAccessToken_WhenUserNull_Throws()
        {
            var sut = CreateSut();
            Assert.Throws<ArgumentNullException>(() => sut.CreateAccessToken(null!));
        }

        [Fact]
        public void CreateAccessToken_WhenUserIdEmpty_Throws()
        {
            var sut = CreateSut();
            var user = new AppUser { Id = Guid.Empty };

            Assert.Throws<ArgumentException>(() => sut.CreateAccessToken(user));
        }
    }
}
