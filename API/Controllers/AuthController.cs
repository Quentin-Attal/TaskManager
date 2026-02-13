using API.Common.Extensions;
using Application.Auth.Interfaces;
using Application.Auth.Services;
using Application.Common;
using Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        {
            var authLoginResult = await _authService.LoginAsync(request, ct);
            if (authLoginResult != null)
            {
                var authResponse = new AuthResponse(authLoginResult.AccessToken, authLoginResult.RefreshTokenPlain, authLoginResult.RefreshTokenExpiresAtUtc);
                var response = ApiResponse<AuthResponse>
                    .SuccessResponse(authResponse, "Login sucessfully");
                return Ok(response);
            }
            else
            {
                var errorResponse = ApiResponse<bool>
                     .FailureResponse("Login failed");
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
                    .SuccessResponse(authResponse, "Login sucessfully");
                return Ok(response);
            }
            else
            {
                var errorResponse = ApiResponse<bool>
                     .FailureResponse("Login failed");
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
