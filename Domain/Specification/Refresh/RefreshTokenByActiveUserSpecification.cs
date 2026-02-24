using Domain.Entities;

namespace Domain.Specification.Refresh
{
    public class RefreshTokenByActiveUserSpecification : Specification<RefreshToken>
    {
        public RefreshTokenByActiveUserSpecification(Guid userId)
            : base(rt => rt.UserId == userId && rt.RevokedAtUtc == null && rt.ExpiresAtUtc > DateTime.UtcNow)
        {
        }
    }
}