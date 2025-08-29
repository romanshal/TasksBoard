using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Authentication.Infrastructure.Security
{
    internal static class TokenHasher
    {
        private const KeyDerivationPrf PRF = KeyDerivationPrf.HMACSHA256;
        private const int ITERATION_COUNT = 100_000;
        private const int NUM_BYTES = 32;

        public static (string Hash, string Salt) Hash(string token)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: token,
                salt: saltBytes,
                prf: PRF,
                iterationCount: ITERATION_COUNT,
                numBytesRequested: NUM_BYTES
            ));

            return (hash, salt);
        }

        public static bool Verify(string token, string hash, string salt)
        {
            var hashToCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: token,
                salt: Convert.FromBase64String(salt),
                prf: PRF,
                iterationCount: ITERATION_COUNT,
                numBytesRequested: NUM_BYTES
            ));

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(hash),
                Convert.FromBase64String(hashToCheck)
            );
        }
    }
}
