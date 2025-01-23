using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
namespace Project_pearl.Controllers
{
    public class PasswordHasher
    {
        public PasswordHasher()
        {

        }
        public static string HashedPassword(string Password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword(string hashedPassword, string Password)
        {
            string hashedInput = HashedPassword(Password);
            return hashedInput == hashedPassword;
        }
    }
}