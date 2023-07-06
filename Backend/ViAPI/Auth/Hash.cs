using System.Security.Cryptography;
using System.Text;

namespace ViAPI.Auth;

public class Hash
{
    public static byte[] GenerateSalt(int saltLength) => RandomNumberGenerator.GetBytes(saltLength);
    public static string GenerateHash(string password, byte[]? salt = null)
    {
        byte[] hash;
        if (salt is not null)
        {
            hash = SHA1.HashData(Encoding.UTF8.GetBytes(password + salt));
        }
        else
        {
            hash = SHA1.HashData(Encoding.UTF8.GetBytes(password));
        }
        string hashString = Convert.ToBase64String(hash);
        return hashString;
    }
    public static bool ValidateHash(string hash1, string hash2) => string.Equals(hash1, hash2);
}