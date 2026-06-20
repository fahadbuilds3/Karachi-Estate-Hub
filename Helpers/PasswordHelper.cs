using System;
using System.Security.Cryptography;
using System.Text;

namespace KarachiEstateHub.Helpers
{
    public static class PasswordHelper
    {
        private const string Prefix = "sha256";

        public static string HashPassword(string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required.", "password");
            }

            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            var salt = Convert.ToBase64String(saltBytes);
            var hash = ComputeHash(password, salt);
            return String.Format("{0}${1}${2}", Prefix, salt, hash);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            var parts = storedHash.Split('$');
            if (parts.Length != 3 || !String.Equals(parts[0], Prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var expectedHash = ComputeHash(password, parts[1]);
            return SlowEquals(expectedHash, parts[2]);
        }

        private static string ComputeHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(salt + password);
                return Convert.ToBase64String(sha256.ComputeHash(bytes));
            }
        }

        private static bool SlowEquals(string left, string right)
        {
            var leftBytes = Encoding.UTF8.GetBytes(left ?? String.Empty);
            var rightBytes = Encoding.UTF8.GetBytes(right ?? String.Empty);
            var diff = leftBytes.Length ^ rightBytes.Length;
            var length = Math.Min(leftBytes.Length, rightBytes.Length);

            for (var i = 0; i < length; i++)
            {
                diff |= leftBytes[i] ^ rightBytes[i];
            }

            return diff == 0;
        }
    }
}
