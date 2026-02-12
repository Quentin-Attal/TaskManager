namespace Infrastructure.Auth.Options
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;

        // Must be long (at least 32+ chars for HMAC SHA256)
        public string Key { get; init; } = default!;

        public int AccessTokenMinutes { get; init; } = 15;
        public int RefreshTokenDays { get; init; } = 30;
    }
}
