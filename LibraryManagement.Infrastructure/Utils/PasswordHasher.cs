using System.Security.Cryptography;
using System.Text;

namespace LibraryManagement.Infrastructure.Utils
{
    public static class PasswordHasher
    {
        public static byte[] GenerateSalt()
        {
            var salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string HashPassword(string password, byte[] salt)
        {
            using var sha256 = SHA256.Create();
            var passwordBytes = Encoding.UTF8.GetBytes(password);

                        var combinedBytes = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, combinedBytes, passwordBytes.Length, salt.Length);

                        var hashBytes = sha256.ComputeHash(combinedBytes);

                        return Convert.ToBase64String(hashBytes);
        }
    }
}