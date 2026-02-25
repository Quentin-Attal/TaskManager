using Domain.Entities;

namespace Domain.Specification.Refresh
{
    public class RefreshTokenByTokenHashSpecification : Specification<RefreshToken>
    {
        public RefreshTokenByTokenHashSpecification(string tokenHash)
            : base(t => t.TokenHash == tokenHash)
        {
            // Mandatory to include User else we can't refresh token
            Includes.Add(rt => rt.User);
        }
    }
}