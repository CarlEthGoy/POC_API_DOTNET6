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
    private readonly IUserRepository _userRepository;
    private readonly ICryptographyUtil _cryptoUtil;

    public BLLAuthentication(IUserRepository repository, ICryptographyUtil cryptoUtil)
    {
      _userRepository = repository;
      _cryptoUtil = cryptoUtil;
    }

    public async Task<bool> Login(string username, string password)
    {
      IUserModel user = await _userRepository.GetUserByUsername(username);
      return _cryptoUtil.VerifyHash(password, user.Salt, user.Hash);

    }
  }

}
