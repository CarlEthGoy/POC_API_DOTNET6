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
    string GenerateRandomPassword(EnumPasswordComplexity complexity);

    byte[] GenerateSalt();

    byte[] HashPassword(string password, byte[] salt);

    bool IsPasswordValid(string password, EnumPasswordComplexity complexity);

    bool VerifyHash(string password, byte[] salt, byte[] hash);
  }

  public class CryptographyUtil : ICryptographyUtil
  {
    // Singleton
    private static readonly Lazy<CryptographyUtil> lazy = new();

    private readonly Dictionary<EnumCharType, Regex> _chars = new(){
            { EnumCharType.Lowercase, new Regex(@"[abcdefghijklmnopqrstuvwxyz]")},
            { EnumCharType.Uppercase, new Regex(@"[ABCDEFGHIJKLMNOPQRSTUVWXYZ]")},
            { EnumCharType.Digit, new Regex(@"[0123456789]")},
            { EnumCharType.Special, new Regex(@"[!@#$%^&*()-_=+{}\[\]?<>.,]")}
    };

    public static CryptographyUtil Instance
    { get { return lazy.Value; } }

    public string GenerateRandomPassword(EnumPasswordComplexity complexity)
    {
      Password pwd = new(true, true, true, true, (int)complexity);
      string password = pwd.Next();
      return password;
    }

    //TODO
    public string EncryptasswordForString(string passwordToEncrypt, EnumPasswordComplexity complexity)
    {
      Password pwd = new(true, true, true, true, (int)complexity);
      string password = pwd.Next();
      return passwordToEncrypt;
    }

    //TODO
    public string DecryptPasswordForString(string passwordToEncrypt, EnumPasswordComplexity complexity)
    {
      Password pwd = new(true, true, true, true, (int)complexity);
      string password = pwd.Next();
      return passwordToEncrypt;
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

    public bool IsPasswordValid(string password, EnumPasswordComplexity complexity)
    {
      bool lowercaseValid = _chars[EnumCharType.Lowercase].IsMatch(password);
      bool uppercaseValid = _chars[EnumCharType.Uppercase].IsMatch(password);
      bool digitValid = _chars[EnumCharType.Digit].IsMatch(password);
      bool specialValid = _chars[EnumCharType.Special].IsMatch(password);
      if (password.Length < (int)complexity)
      {
        return false;
      }

      return lowercaseValid && uppercaseValid && digitValid && specialValid;
    }

    public bool VerifyHash(string password, byte[] salt, byte[] hash)
    {
      byte[] newHash = HashPassword(password, salt);
      return hash.SequenceEqual(newHash);
    }
  }
}