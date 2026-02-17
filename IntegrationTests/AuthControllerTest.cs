using API.Common.Serialization;
using Application.Common;
using Contracts.Auth;
using Contracts.Tasks;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class AuthControllerTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly Guid _taskId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public AuthControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;

            _client.DefaultRequestHeaders.Add("x-test-userid", _userId.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            await ValueTask.CompletedTask;
            GC.SuppressFinalize(this);
        }

        public async ValueTask InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            string password = "hash";
            var hasherMock = new Mock<PasswordHasher<AppUser>>();

            var hasher = new PasswordHasher<AppUser>();

            var user = new AppUser
            {
                Id = _userId,
                Email = "email@mail.com",
                CreatedAtUtc = DateTime.UtcNow,
            };
            user.PasswordHash = hasher.HashPassword(user, password);

            db.Users.Add(user);

            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task Get_Login_Should_Return_200()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var loginRequest = new LoginRequest("email@mail.com", "hash");
            var response = await _client.SendAsync(PostJson("/api/auth/login", loginRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.AccessToken.Should().NotBe(null);
            apiResponse.Data.ExpirationDate.Should().NotBe(null);
            apiResponse.Message.Should().Be("Login sucessfully");
        }

        [Fact]
        public async Task Get_Login_Should_Return_401()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var loginRequest = new LoginRequest("email@mail.com", "haash");
            var response = await _client.SendAsync(PostJson("/api/auth/login", loginRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeFalse();
            apiResponse.Message.Should().Be("Login failed");
        }

        [Fact]
        public async Task Get_Register_Should_Return_200()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var registerRequest = new RegisterRequest("newEmail@mail.com", "hashPasword12@", "hashPasword12@");
            var response = await _client.SendAsync(PostJson("/api/auth/register", registerRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.AccessToken.Should().NotBe(null);
            apiResponse.Data.ExpirationDate.Should().NotBe(null);
            apiResponse.Message.Should().Be("Register sucessfully");
        }

        [Fact]
        public async Task Get_Register_Wrong_Confirm_Password_Should_Return_400()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var registerRequest = new RegisterRequest("newEmail@mail.com", "hashPasword12@", "haashPasword12@");
            var response = await _client.SendAsync(PostJson("/api/auth/register", registerRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeFalse();
            apiResponse.Message.Should().Be("Password not match");
        }

        [Fact]
        public async Task Get_Register_Wrong_Password_Weak_Should_Return_400()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var registerRequest = new RegisterRequest("newEmail@mail.com", "hash", "haash");
            var response = await _client.SendAsync(PostJson("/api/auth/register", registerRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeFalse();
            apiResponse.Message.Should().Be("Password too weak");
        }

        [Fact]
        public async Task Get_Register_Email_Format_Should_Return_400()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var registerRequest = new RegisterRequest("mail.com", "hashPasword12@", "hashPasword12@");
            var response = await _client.SendAsync(PostJson("/api/auth/register", registerRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeFalse();
            apiResponse.Message.Should().Be("Format email is wrong");
        }

        [Fact]
        public async Task Get_Register_Email_Exist_Should_Return_400()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var registerRequest = new RegisterRequest("email@mail.com", "hashPasword12@", "hashPasword12@");
            var response = await _client.SendAsync(PostJson("/api/auth/register", registerRequest), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeFalse();
            apiResponse.Message.Should().Be("Email alredy exist");
        }

        [Fact]
        public async Task Login_Should_Return_429_When_Too_Many_Requests()
        {
            var ct = TestContext.Current.CancellationToken;
            var loginRequest = new LoginRequest("email@mail.com", "haash");

            var r1 = await _client.PostAsJsonAsync("/api/auth/login", loginRequest, cancellationToken: ct);
            r1.StatusCode.Should().NotBe(HttpStatusCode.TooManyRequests);

            var r2 = await _client.PostAsJsonAsync("/api/auth/login", loginRequest, cancellationToken: ct);
            r2.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task Post_Refresh_Should_Return_200_When_Cookie_Is_Valid()
        {
            var ct = TestContext.Current.CancellationToken;

            var loginRequest = new LoginRequest("email@mail.com", "hash");
            var loginResponse = await _client.SendAsync(PostJson("/api/auth/login", loginRequest), cancellationToken: ct);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var setCookie = loginResponse.Headers.TryGetValues("Set-Cookie", out var values)
                ? values.FirstOrDefault(v => v.StartsWith("refresh_token="))
                : null;

            setCookie.Should().NotBeNull();

            using var refreshMessage = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");
            refreshMessage.Headers.Add("Cookie", setCookie!.Split(';')[0]);

            var refreshResponse = await _client.SendAsync(refreshMessage, ct);
            refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var refreshApi = await refreshResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(cancellationToken: ct);
            refreshApi.Should().NotBeNull();
            refreshApi!.Success.Should().BeTrue();
            refreshApi.Data.Should().NotBeNull();
            refreshApi.Data!.AccessToken.Should().NotBeNull();

            refreshApi.Data.ExpirationDate.Should().BeNull();
            refreshApi.Message.Should().Be("Refresh sucessfully");
        }

        [Fact]
        public async Task Post_Refresh_Should_Return_400_When_Cookie_Missing()
        {
            var ct = TestContext.Current.CancellationToken;

            var response = await _client.PostAsync("/api/auth/refresh", content: null, cancellationToken: ct);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_Refresh_Should_Return_404_When_Cookie_Invalid()
        {
            var ct = TestContext.Current.CancellationToken;

            using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");
            msg.Headers.Add("Cookie", "refresh_token=invalid_token_value");

            var response = await _client.SendAsync(msg, ct);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var api = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: ct);
            api.Should().NotBeNull();
            api!.Success.Should().BeFalse();
            api.Message.Should().Be("Refresh failed");
        }

        [Fact]
        public async Task Post_Logout_Should_Return_200_And_Delete_Cookie_When_Cookie_Present()
        {
            var ct = TestContext.Current.CancellationToken;

            using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
            msg.Headers.Add("Cookie", "refresh_token=some_refresh_token");

            var response = await _client.SendAsync(msg, ct);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders).Should().BeTrue();
            var setCookie = setCookieHeaders!.ToList();

            setCookie.Any(h => h.StartsWith("refresh_token=", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            setCookie.Any(h =>
                h.Contains("expires=", StringComparison.OrdinalIgnoreCase) ||
                h.Contains("max-age=0", StringComparison.OrdinalIgnoreCase)
            ).Should().BeTrue();
        }

        private HttpRequestMessage PostJson<T>(string url, T body, string? rateKey = null)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, url);
            msg.Headers.Add("x-test-userid", _userId.ToString());
            msg.Headers.Add("x-test-ratelimit-key", rateKey ?? Guid.NewGuid().ToString()); // ✅ unique = pas de 429
            msg.Content = JsonContent.Create(body);
            return msg;
        }
    }
}
