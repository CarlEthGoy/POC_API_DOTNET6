using API.Enum;
using Konscious.Security.Cryptography;
using PasswordGenerator;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Cryptography
{

  public static class CryptographyUtil
  {
    private static readonly Dictionary<EnumCharType, Regex> _chars = new Dictionary<EnumCharType, Regex>()
        {
            { EnumCharType.Lowercase, new Regex(@"[abcdefghijklmnopqrstuvwxyz]")},
            { EnumCharType.Uppercase, new Regex(@"[ABCDEFGHIJKLMNOPQRSTUVWXYZ]")},
            { EnumCharType.Digit, new Regex(@"[0123456789]")},
            { EnumCharType.Special, new Regex(@"[!@#$%^&*()-_=+{}[]?<>.,]")}
        };

    public static bool IsPasswordValid(string password, EnumPasswordComplexity complexity)
    {
      var lowercaseValid = _chars[EnumCharType.Lowercase].IsMatch(password);
      var uppercaseValid = _chars[EnumCharType.Uppercase].IsMatch(password);
      var digitValid = _chars[EnumCharType.Digit].IsMatch(password);
      var specialValid = _chars[EnumCharType.Special].IsMatch(password);
      if (password.Length >= (int)complexity)
      {

      }

      return lowercaseValid && uppercaseValid && digitValid && specialValid;
    }

    public static string GenerateRandomPassword(EnumPasswordComplexity complexity)
    {
      Password pwd = new(true, true, true, true, (int)complexity);
      string password = pwd.Next();
      return password;
    }

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