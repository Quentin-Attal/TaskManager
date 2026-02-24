using Domain.Entities;

namespace Domain.Specification.Refresh
{
    public class RefreshTokenByTokenHashAndUserIdSpecification : Specification<RefreshToken>
    {
        public RefreshTokenByTokenHashAndUserIdSpecification(Guid userId, string tokenHash)
            : base(t => t.UserId == userId && t.TokenHash == tokenHash)
        {
            Includes.Add(rt => rt.User);
        }
    }
}