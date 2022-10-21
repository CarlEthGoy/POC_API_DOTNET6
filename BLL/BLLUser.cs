using API.Cryptography;
using API.Database;
using API.Enum;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLUser
  {
    Task<int?> CreateUser(IUserViewModel userViewModel);
    Task<bool> DeleteUserById(int id);
    Task<IUserModel?> GetUserById(int id);
    Task<IUserModel?> GetByUsername(string username);
  }

  public class BLLUser : IBLLUser
  {
    private readonly IUserRepository _userRepository;

    public BLLUser(IUserRepository repository)
    {
      _userRepository = repository;
    }

    public async Task<IUserModel?> GetUserById(int id)
    {
      var user = await _userRepository.GetUserById(id);
      return user;
    }

    public async Task<int?> CreateUser(IUserViewModel userViewModel)
    {
      bool isPasswordValid = CryptographyUtil.Instance.IsPasswordValid(userViewModel.Password, EnumPasswordComplexity.Medium);
      if (!isPasswordValid)
      {
        throw new Exception("Password is invalid.");
      }

      IUserModel userModel = _userRepository.GenerateUserToCreate(userViewModel);

      // Valider que le username n'existe pas!
      if (await _userRepository.IsUsernameAlreadyUsed(userViewModel.Username))
      {
        throw new Exception($"This username is already in use : {userViewModel.Username}");
      }

      int? createdUserId = await _userRepository.CreateUser(userModel);
      return createdUserId;
    }

    public async Task<bool> DeleteUserById(int id)
    {
      return await _userRepository.DeleteUserById(id);
    }

    public async Task<IUserModel?> GetByUsername(string username)
    {
      var user = await _userRepository.GetUserByUsername(username);
      return user;
    }

  }
}
