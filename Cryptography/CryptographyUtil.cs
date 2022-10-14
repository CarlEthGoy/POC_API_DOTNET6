using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace API.Cryptography
{
  public static class CryptographyUtil
  {
    public static byte[] GenerateSalt()
    {
      var buffer = new byte[16];
      var rng = RandomNumberGenerator.Create();
      rng.GetBytes(buffer);
      return buffer;
    }

    public static byte[] HashPassword(string password, byte[] salt)
    {
      var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

      argon2.Salt = salt;
      argon2.DegreeOfParallelism = 8; // four cores
      argon2.Iterations = 4;
      argon2.MemorySize = 1024 * 1024; // 1 GB

      return argon2.GetBytes(16);
    }

    public static bool VerifyHash(string password, byte[] salt, byte[] hash)
    {
      var newHash = HashPassword(password, salt);
      return hash.SequenceEqual(newHash);
    }
  }
}