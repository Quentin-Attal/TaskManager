using Contracts.Auth;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<AppUser> _hasher = new();

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

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
            var refreshPlain = Request.Cookies["refresh_token"];


            Response.Cookies.Delete("refresh_token");
            return Ok();
        }

    }
}
