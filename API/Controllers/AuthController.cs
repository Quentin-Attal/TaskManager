using API.Common.Extensions;
using Application.Auth.Interfaces;
using Application.Auth.Models;
using Application.Auth.Services;
using Application.Common;
using Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        {
            var (Result, Error) = await _authService.LoginAsync(request, ct);
            if (Result != null)
            {
                var result = Result;
                Response.Cookies.Append(
                    "refresh_token",
                    result.RefreshTokenPlain,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Path = "/api/auth",
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    }
                );
                var authResponse = new AuthResponse(result.AccessToken, result.RefreshTokenExpiresAtUtc);
                var response = ApiResponse<AuthResponse>
                    .SuccessResponse(authResponse, "Login sucessfully");
                return Ok(response);
            }
            else
            {
                string message = Error switch
                {
                    AuthErrorCode.InvalidEmail => "Format email is wrong",
                    _ => "Login failed",
                };
                var errorResponse = ApiResponse<bool>
                     .FailureResponse(message);
                return Unauthorized(errorResponse);
            }
        }

        [EnableRateLimiting("auth")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
        {
            var (Result, Error) = await _authService.RegisterAsync(request, ct);
            if (Result != null)
            {
                var result = Result;
                Response.Cookies.Append(
                    "refresh_token",
                    result.RefreshTokenPlain,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Path = "/api/auth",
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    }
                );
                var authResponse = new AuthResponse(result.AccessToken, result.RefreshTokenExpiresAtUtc);
                var response = ApiResponse<AuthResponse>
                    .SuccessResponse(authResponse, "Register sucessfully");
                return Ok(response);
            }
            else
            {
                string message = Error switch
                {
                    AuthErrorCode.InvalidEmail => "Format email is wrong",
                    AuthErrorCode.InvalidCredentials => "Password not match",
                    AuthErrorCode.PasswordTooWeak => "Password too weak",
                    AuthErrorCode.EmailAlreadyExists => "Email already exist",
                    _ => "Register failed",
                };
                var errorResponse = ApiResponse<bool>
                     .FailureResponse(message);
                return BadRequest(errorResponse);
            }
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                var errorResponse = ApiResponse<bool>.FailureResponse("Missing refresh token.");
                return Unauthorized(errorResponse);
            }

            var authRefreshResult = await _authService.RefreshAsync(refreshToken, ct);
            if (authRefreshResult != null)
            {
                var authResponse = new AuthResponse(authRefreshResult.AccessToken, null);
                var response = ApiResponse<AuthResponse>
                    .SuccessResponse(authResponse, "Refresh sucessfully");
                return Ok(response);
            }
            else
            {
                var errorResponse = ApiResponse<bool>
                     .FailureResponse("Refresh failed");
                return Unauthorized(errorResponse);
            }
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = User.GetUserId();
            await _authService.LogoutAsync(userId, ct);

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                Path = "/api/auth",
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax
            });

            return Ok();
        }

    }
}
