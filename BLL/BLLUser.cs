using API.Core;
using API.Cryptography;
using API.Database;
using API.Enum;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLUser
  {
    public Task<string> CreateUser(UserViewModel userViewModel);

    public Task<UserModel> GetById(int id);

    public Task<UserModel> GetByUsername(string username);
  }

  public class BLLUser : MustInitialize<IUserRepository>, IBLLUser
  {
    private IUserRepository _repository;

    public BLLUser(IUserRepository repository) : base(repository)
    {
      _repository = repository;
    }

    public async Task<string> CreateUser(UserViewModel userViewModel)
    {
      bool isPasswordValid = CryptographyUtil.IsPasswordValid(userViewModel.password, EnumPasswordComplexity.Medium);
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
