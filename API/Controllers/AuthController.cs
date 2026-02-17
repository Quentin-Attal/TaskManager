using API.Common.Extensions;
using Application.Auth.Interfaces;
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
                var authResponse = new AuthResponse(result.AccessToken, result.RefreshTokenPlain, result.RefreshTokenExpiresAtUtc);
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
                var authResponse = new AuthResponse(result.AccessToken, result.RefreshTokenPlain, result.RefreshTokenExpiresAtUtc);
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
                    AuthErrorCode.EmailAlreadyExists => "Email alredy exist",
                    _ => "Register failed",
                };
                var errorResponse = ApiResponse<bool>
                     .FailureResponse(message);
                return BadRequest(errorResponse);
            }
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var refreshToken = Request.Cookies["refresh_token"];
            if (refreshToken == null)
            {
                return BadRequest();
            }
            var authRefreshResult = await _authService.RefreshAsync(userId, refreshToken, ct);
            if (authRefreshResult != null)
            {
                var authResponse = new AuthResponse(authRefreshResult.AccessToken, null, null);
                var response = ApiResponse<AuthResponse>
                    .SuccessResponse(authResponse, "Refresh sucessfully");
                return Ok(response);
            }
            else
            {
                var errorResponse = ApiResponse<bool>
                     .FailureResponse("Refresh failed");
                return NotFound(errorResponse);
            }

        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var refreshToken = Request.Cookies["refresh_token"];
            if (refreshToken == null)
            {
                return Ok();
            }
            await _authService.LogoutAsync(userId, refreshToken, ct);

            Response.Cookies.Delete("refresh_token");
            return Ok();
        }

    }
}
