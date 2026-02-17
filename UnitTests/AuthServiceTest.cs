using Application.Auth.Interfaces;
using Application.Auth.Models;
using Application.Auth.Services;
using Application.Repositories;
using Application.Tasks.Services;
using Contracts.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTests
{
    public class AuthServiceTest
    {
        [Fact]
        public async Task Should_LoginAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            string email = "email@mail.com";
            string password = "password12@";
            var loginRequest = new LoginRequest(email, password);

            var repoUserMock = new Mock<IUserRepository>();
            var repoRefreshTokenMock = new Mock<IRefreshTokenRepository>();
            var serviceTokenMock = new Mock<ITokenService>();
            var hasherMock = new Mock<PasswordHasher<AppUser>>();

            var hasher = new PasswordHasher<AppUser>();

            var user = new AppUser
            {
                Email = email
            };
            user.PasswordHash = hasher.HashPassword(user, password);

            repoUserMock
                .Setup(r => r.GetByEmailAsync(email, cancellationToken))
                .ReturnsAsync(user);

            serviceTokenMock
                .Setup(s => s.CreateAccessToken(It.IsAny<AppUser>()))
                .Returns("access-token");

            // Whatever your refresh token return type is—adjust accordingly
            serviceTokenMock
                .Setup(s => s.CreateRefreshToken())
                .Returns(new RefreshTokenDescriptor("plain", "hash", DateTime.UtcNow.AddDays(7)));

            hasherMock
                .Setup(h => h.VerifyHashedPassword(
                    It.IsAny<AppUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            var handler = new AuthService(repoUserMock.Object, repoRefreshTokenMock.Object, serviceTokenMock.Object);

            var result = await handler.LoginAsync(loginRequest, cancellationToken);
            Assert.IsType<(AuthLoginResult, AuthErrorCode)>(result, exactMatch: true);
            repoUserMock.Verify(r => r.GetByEmailAsync(email, cancellationToken), Times.Once);

            serviceTokenMock.Verify(r => r.CreateAccessToken(It.IsAny<AppUser>()), Times.Once);
            serviceTokenMock.Verify(r => r.CreateRefreshToken(), Times.Once);

            repoRefreshTokenMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), cancellationToken), Times.Once);
            repoRefreshTokenMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        }


        [Fact]
        public async Task Should_RegisterAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            string email = "newemail@mail.com";
            string password = "hashPasword12@";
            string confirmPassword = "hashPasword12@";
            var loginRequest = new RegisterRequest(email, password, confirmPassword);

            var repoUserMock = new Mock<IUserRepository>();
            var repoRefreshTokenMock = new Mock<IRefreshTokenRepository>();
            var serviceTokenMock = new Mock<ITokenService>();
            var hasherMock = new Mock<PasswordHasher<AppUser>>();

            var hasher = new PasswordHasher<AppUser>();


            var user = new AppUser
            {
                Email = email
            };
            user.PasswordHash = hasher.HashPassword(user, password);

            repoUserMock
                .Setup(r => r.GetByEmailAsync(email, cancellationToken))
                .ReturnsAsync((AppUser?)null);

            serviceTokenMock
                .Setup(s => s.CreateAccessToken(It.IsAny<AppUser>()))
                .Returns("access-token");

            var handler = new AuthService(repoUserMock.Object, repoRefreshTokenMock.Object, serviceTokenMock.Object);

            var result = await handler.RegisterAsync(loginRequest, cancellationToken);
            Assert.IsType<(AuthLoginResult, AuthErrorCode)>(result, exactMatch: true);
            repoUserMock.Verify(r => r.GetByEmailAsync(email, cancellationToken), Times.Exactly(2));
            repoUserMock.Verify(r => r.AddAsync(It.IsAny<AppUser>(), cancellationToken), Times.Once);
            repoUserMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

        }

        [Fact]
        public async Task Should_LogoutAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();
            var hash = "hash";

            var repoUserMock = new Mock<IUserRepository>();
            var repoRefreshTokenMock = new Mock<IRefreshTokenRepository>();
            var serviceTokenMock = new Mock<ITokenService>();
            var hasherMock = new Mock<PasswordHasher<AppUser>>();

            var hasher = new PasswordHasher<AppUser>();

            var token = new RefreshToken()
            {
                TokenHash = hash,
                UserId = userId,
            };

            // Whatever your refresh token return type is—adjust accordingly
            serviceTokenMock
                .Setup(s => s.HashRefreshToken(hash))
                .Returns(hash);

            repoRefreshTokenMock
                .Setup(s => s.FindByHashAsync(userId, hash, cancellationToken))
                .ReturnsAsync(token);


            var handler = new AuthService(repoUserMock.Object, repoRefreshTokenMock.Object, serviceTokenMock.Object);

            await handler.LogoutAsync(userId, hash, cancellationToken);

            serviceTokenMock.Verify(r => r.HashRefreshToken(hash), Times.Once);

            repoRefreshTokenMock.Verify(r => r.FindByHashAsync(userId, hash, cancellationToken), Times.Once);
            repoRefreshTokenMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RefreshAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();
            var hash = "hash";
            string email = "email@mail.com";

            var repoUserMock = new Mock<IUserRepository>();
            var repoRefreshTokenMock = new Mock<IRefreshTokenRepository>();
            var serviceTokenMock = new Mock<ITokenService>();
            var hasherMock = new Mock<PasswordHasher<AppUser>>();

            var hasher = new PasswordHasher<AppUser>();

            var token = new RefreshToken()
            {
                TokenHash = hash,
                UserId = userId,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(2),
            };

            var user = new AppUser
            {
                Email = email,
                Id = userId
            };

            // Whatever your refresh token return type is—adjust accordingly
            repoRefreshTokenMock
                .Setup(s => s.FindByHashAsync(userId, hash, cancellationToken))
                .ReturnsAsync(token);

            serviceTokenMock
                .Setup(s => s.HashRefreshToken(hash))
                .Returns(hash);

            serviceTokenMock
                .Setup(s => s.CreateRefreshToken())
                .Returns(new RefreshTokenDescriptor("token", "hash", new DateTime().AddDays(7)));


            repoUserMock
                .Setup(s => s.GetByIdAsync(userId, cancellationToken))
                .ReturnsAsync(user);


            var handler = new AuthService(repoUserMock.Object, repoRefreshTokenMock.Object, serviceTokenMock.Object);

            var result = await handler.RefreshAsync(userId, hash, cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<AuthRefreshResult>(result, exactMatch: true);

            serviceTokenMock.Verify(r => r.HashRefreshToken(hash), Times.Once);
            serviceTokenMock.Verify(r => r.CreateAccessToken(user), Times.Once);

            repoRefreshTokenMock.Verify(r => r.FindByHashAsync(userId, hash, cancellationToken), Times.Once);
            repoRefreshTokenMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Should_QueryUserByNormalizedEmail()
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenRepo = new Mock<IRefreshTokenRepository>(MockBehavior.Loose);
            var tokenService = new Mock<ITokenService>(MockBehavior.Loose);

            var ct = CancellationToken.None;
            var req = new LoginRequest("  TeSt@Example.COM  ", "whatever");

            // Expect normalized email
            userRepo
                .Setup(r => r.GetByEmailAsync("test@example.com", ct))
                .ReturnsAsync((AppUser?)null);

            var sut = new AuthService(userRepo.Object, tokenRepo.Object, tokenService.Object);

            // Act
            await sut.LoginAsync(req, ct);

            // Assert
            userRepo.VerifyAll();
        }
    }
}
