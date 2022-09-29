using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace CeskyRozhlasMiner.WebApp.Data.Utilities
{
    /// <summary>
    /// Utility class for creating/reconstructing password hash.
    /// </summary>
    public class PasswordHashFactory
    {
        private readonly string _password;
        private readonly byte[] _salt;

        /// <summary>
        /// Initializes factory.
        /// </summary>
        /// <param name="password">Password as a plain text.</param>
        /// <param name="usedSalt">If null then a new salt is generated, otherwise
        /// previously generated and serialized salt will be used.</param>
        public PasswordHashFactory(string password, string usedSalt = null)
        {
            _password = password;
            _salt = usedSalt == null ? GenerateNewSalt() : Convert.FromBase64String(usedSalt);
        }

        /// <summary>
        /// Generates a new salt.
        /// </summary>
        /// <returns>Sequence of bytes representing salt to be used in derivation process.</returns>
        public static byte[] GenerateNewSalt()
        {
            byte[] result = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(result);
            }

            return result;
        }

        /// <summary>
        /// Gets salt in the serialized form.
        /// </summary>
        /// <returns>Serialized sequence of bytes representing salt.</returns>
        public string GetSerializedSalt()
        {
            return Convert.ToBase64String(_salt);
        }

        /// <summary>
        /// Gets password hash generated using Pbkdf2-HMACSHA1 using specified salt.
        /// </summary>
        /// <returns>Generated password hash.</returns>
        public string GetPasswordHash()
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: _password,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

    }
}
