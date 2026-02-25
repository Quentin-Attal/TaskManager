using Domain.Exceptions;

namespace Domain.Entities
{
    public class RefreshToken
    {

        private RefreshToken() { }
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }
        public AppUser User { get; private set; } = null!;

        // token is hash for better security
        public string TokenHash { get; private set; } = default!;

        public DateTime CreatedAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }

        public DateTime? RevokedAtUtc { get; private set; }
        public string? ReplacedByTokenHash { get; private set; }

        public static RefreshToken Create(Guid userId, string tokenHash, DateTime nowUtc, DateTime expiresAtUtc)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty.");

            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new DomainException("TokenHash cannot be empty.");

            if (expiresAtUtc <= nowUtc)
                throw new DomainException("Refresh token expiry must be in the future.");

            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAtUtc = nowUtc,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        public bool IsExpired(DateTime nowUtc) => nowUtc >= ExpiresAtUtc;
        public bool IsActive(DateTime nowUtc) => RevokedAtUtc is null && !IsExpired(nowUtc);

        public void Revoke(DateTime nowUtc, string? replacedByTokenHash = null)
        {
            if (replacedByTokenHash is not null && string.IsNullOrWhiteSpace(replacedByTokenHash))
                throw new DomainException("ReplacedByTokenHash cannot be empty.");

            if (RevokedAtUtc is not null)
            {
                if (replacedByTokenHash is not null && ReplacedByTokenHash is not null && replacedByTokenHash != ReplacedByTokenHash)
                {
                    throw new DomainException("Token already revoked with different replacement");
                }
                return;
            }
            RevokedAtUtc = nowUtc;
            ReplacedByTokenHash = replacedByTokenHash;
        }
    }
}
