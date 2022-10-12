using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Text;

namespace API.Cryptography
{
  public class CryptographyUtil
  {
    private readonly IConfiguration _configuration;
    private readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public CryptographyUtil(IConfiguration configuration)
    {
      _configuration = configuration;
    }


    public string? HashPassword(string password)
    {
      var passwordHash = Argon2.Hash(password);
      if (Argon2.Verify(passwordHash, password))
      {
        return passwordHash;
      }

      return "";
    }

    public bool VerifyHash(string hashString, string password)
    {
      byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
      if (Argon2.Verify(hashString, passwordBytes, 5))
      {
        return true;
      }

      return false;
    }
  }
}