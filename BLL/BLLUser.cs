using API.Cryptography;
using API.Database;
using API.Models.V1;

namespace API.BLL
{
    public class BLLUser
  {
    private IUserRepository _repository;

    public BLLUser(IUserRepository repository)
    {
      _repository = repository;
    }

    public async Task<string> CreateUser(UserViewModel userViewModel)
    {
      bool isPasswordValid = CryptographyUtil.IsPasswordValid(userViewModel.password, CryptographyUtil.EnumPassworrdComplexity.Medium);
      if (!isPasswordValid)
      {
        throw new Exception("Password is invalid.");
      }

      byte[] salt = CryptographyUtil.GenerateSalt();

      UserModel userModel = new UserModel
      {
        username = userViewModel.username,
        hash = CryptographyUtil.HashPassword(userViewModel.password, salt),
        name = userViewModel.name,
        salt = salt,
      };

      string createdUserId = await _repository.CreateUser(userModel);
      return createdUserId;
    }

    public async Task<UserModel> GetById(int id)
    {
      UserModel createdUser = await _repository.GetById(id);
      return createdUser;
    }

    public async Task<UserModel> GetByUsername(string username)
    {
      UserModel createdUser = await _repository.GetByUsername(username);
      return createdUser;
    }

  }
}
