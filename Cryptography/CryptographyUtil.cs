using API.Enum;
using Konscious.Security.Cryptography;
using PasswordGenerator;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Cryptography
{
  public interface ICryptographyUtil
  {
    bool IsPasswordValid(string password, EnumPasswordComplexity complexity);
    string GenerateRandomPassword(EnumPasswordComplexity complexity);
    byte[] GenerateSalt();
    byte[] HashPassword(string password, byte[] salt);
    bool VerifyHash(string password, byte[] salt, byte[] hash);

  }

  public class CryptographyUtil : ICryptographyUtil
  {

    private static readonly Lazy<CryptographyUtil> lazy = new();

    public static CryptographyUtil Instance { get { return lazy.Value; } }

    public CryptographyUtil()
    {
    }

    private readonly Dictionary<EnumCharType, Regex> _chars = new()
        {
            { EnumCharType.Lowercase, new Regex(@"[abcdefghijklmnopqrstuvwxyz]")},
            { EnumCharType.Uppercase, new Regex(@"[ABCDEFGHIJKLMNOPQRSTUVWXYZ]")},
            { EnumCharType.Digit, new Regex(@"[0123456789]")},
            { EnumCharType.Special, new Regex(@"[!@#$%^&*()-_=+{}\[\]?<>.,]")}
        };

    public bool IsPasswordValid(string password, EnumPasswordComplexity complexity)
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

    public string GenerateRandomPassword(EnumPasswordComplexity complexity)
    {
      Password pwd = new(true, true, true, true, (int)complexity);
      string password = pwd.Next();
      return password;
    }

    public byte[] GenerateSalt()
    {
      var buffer = new byte[16];
      var rng = RandomNumberGenerator.Create();
      rng.GetBytes(buffer);
      return buffer;
    }

    public byte[] HashPassword(string password, byte[] salt)
    {
      Argon2id argon2 = new(Encoding.UTF8.GetBytes(password))
      {
        Salt = salt,
        DegreeOfParallelism = 8, // four cores
        Iterations = 4,
        MemorySize = 1024 * 1024 // 1 GB
      };

      return argon2.GetBytes(16);
    }

    public bool VerifyHash(string password, byte[] salt, byte[] hash)
    {
      var newHash = HashPassword(password, salt);
      return hash.SequenceEqual(newHash);
    }
  }
}