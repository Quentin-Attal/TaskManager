using Domain.Entities;

namespace Domain.Specification.Refresh
{
    public class RefreshTokenByActiveUserSpecification(Guid userId) : Specification<RefreshToken>(rt => rt.UserId == userId && rt.RevokedAtUtc == null && rt.ExpiresAtUtc > DateTime.UtcNow)
    {
    }
}