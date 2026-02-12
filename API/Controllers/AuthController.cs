using Contracts.Auth;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly IConfiguration _config = config;
        private readonly PasswordHasher<AppUser> _hasher = new();

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        // Rotation: on consomme l'ancien refresh, on en émet un nouveau
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken ct)
        {
            throw new NotImplementedException();

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            _ = Request.Cookies["refresh_token"];


            Response.Cookies.Delete("refresh_token");
            return Ok();
        }

    }
}
