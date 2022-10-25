using API.Cryptography;
using API.Database;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLAuthentication
  {
    Task<bool> Login(string username, string password);
  }

  public class BLLAuthentication : IBLLAuthentication
  {
    private readonly ICryptographyUtil _cryptoUtil;
    private readonly IUserRepository _userRepository;
    public BLLAuthentication(IUserRepository repository, ICryptographyUtil cryptoUtil)
    {
      _userRepository = repository;
      _cryptoUtil = cryptoUtil;
    }

    public async Task<bool> Login(string username, string password)
    {
      IUserModel user = await _userRepository.GetUserByUsername(username);
      bool isVerified = _cryptoUtil.VerifyHash(password, user.Salt, user.Hash);
      return isVerified;

    }
  }

}
