using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace API.Common.Extensions
{

    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id =
                user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(id))
                throw new UnauthorizedAccessException("User id claim is missing.");

            return Guid.Parse(id);
        }
    }

}
