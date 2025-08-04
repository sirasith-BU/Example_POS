using System.Security.Cryptography;
using System.Text;

namespace Example_POS.Helper
{
    public class PasswordHelper
    {
        // สร้าง Salt แบบสุ่ม (32 bytes)
        public static string GenerateSalt(int size = 32)
        {
            var rng = new RNGCryptoServiceProvider();
            var saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        // ทำ Hash + Salt
        public static string HashPassword(string password, string salt)
        {
            var sha256 = SHA256.Create();
            var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }

        // เปรียบเทียบรหัสผ่าน
        public static bool VerifyPassword(string inputPassword, string storedSalt, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword, storedSalt);
            return hashOfInput == storedHash;
        }
    }
}
