using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Auth
{
    public static class TokenUtils
    {
        public static string NewRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Base64UrlEncode(bytes);
        }

        public static string HashRefreshToken(string token, string pepper)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty.", nameof(token));

            var input = $"{token}:{pepper}";
            var bytes = Encoding.UTF8.GetBytes(input);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash); // stored in DB
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
