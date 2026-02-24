using Domain.Entities;

namespace Domain.Specification.Refresh
{
    public class RefreshTokenByTokenHashSpecification : Specification<RefreshToken>
    {
        public RefreshTokenByTokenHashSpecification(string tokenHash)
            : base(t => t.TokenHash == tokenHash)
        {
            Includes.Add(rt => rt.User);
        }
    }
}