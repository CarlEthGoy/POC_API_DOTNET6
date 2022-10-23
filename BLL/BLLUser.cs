using API.Cryptography;
using API.Database;
using API.Enum;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLUser
  {
    Task<int> CreateUser(IUserViewModel userViewModel);
    Task<bool> DeleteUserById(int id);
    Task<IUserModel> GetByUsername(string username);
    Task<IUserModel> GetUserById(int id);
  }

  public class BLLUser : IBLLUser
  {
    private readonly IUserRepository _userRepository;

    public BLLUser(IUserRepository repository)
    {
      _userRepository = repository;
    }

    public async Task<int> CreateUser(IUserViewModel userToCreate)
    {
      if (string.IsNullOrWhiteSpace(userToCreate.Username))
      {
        throw new Exception("Username is required.");
      }

      bool isPasswordValid = CryptographyUtil.Instance.IsPasswordValid(userToCreate.Password, EnumPasswordComplexity.Medium);
      if (!isPasswordValid)
      {
        throw new Exception("Password is invalid.");
      }

      if (await _userRepository.IsUsernameAlreadyUsed(userToCreate.Username))
      {
        throw new Exception($"Username is already in use.");
      }

      IUserModel userModel = _userRepository.GenerateUserModelFromUserViewModel(userToCreate);

      int createdUserId = await _userRepository.CreateUser(userModel);
      return createdUserId;

    }

    public async Task<bool> DeleteUserById(int id)
    {
      return await _userRepository.DeleteUserById(id);
    }

    public async Task<IUserModel> GetByUsername(string username)
    {
      var user = await _userRepository.GetUserByUsername(username);
      return user;
    }

    public async Task<IUserModel> GetUserById(int user_id)
    {
      var user = await _userRepository.GetUserById(user_id);
      return user;
    }
  }
}
