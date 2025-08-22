using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Authentication.Infrastructure.Security
{
    internal static class TokenHasher
    {
        public static (string Hash, string Salt) Hash(string token)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: token,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            ));

            return (hash, salt);
        }

        public static bool Verify(string token, string hash, string salt)
        {
            var hashToCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: token,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            ));

            return hashToCheck == hash;
        }
    }
}
