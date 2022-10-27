using API.Cryptography;
using API.Database;
using API.Enum;
using API.Helper;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLUser
  {
    Task<int> CreateUser(IUserViewModel userViewModel);

    Task<bool> DeleteUserById(int id);

    Task<IUserModel?> GetByUsername(string username);

    Task<IUserModel?> GetUserById(int id);

    Task<bool> PatchUser(int id, UserViewModel userToUpdate);

    Task<bool> PatchUserPassword(int id, string currentPassword, string newPassword);
  }

  public class BLLUser : IBLLUser
  {
    private readonly IUserRepository _userRepository;
    private readonly ICryptographyUtil _cryptographyUtil;
    private readonly IAuthorizationHelper _authorizationHelper;

    public BLLUser(IUserRepository repository, ICryptographyUtil cryptographyUtil, IAuthorizationHelper authorizationHelper)
    {
      _userRepository = repository;
      _cryptographyUtil = cryptographyUtil;
      _authorizationHelper = authorizationHelper;
    }

    public async Task<int> CreateUser(IUserViewModel userToCreate)
    {
      if (string.IsNullOrWhiteSpace(userToCreate.Username))
      {
        throw new Exception("Username is required.");
      }

      bool isPasswordInvalid = !CryptographyUtil.Instance.IsPasswordValid(userToCreate.Password, EnumPasswordComplexity.Medium);
      if (isPasswordInvalid)
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
      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(id))
      {
        return false;
      }

      return await _userRepository.DeleteUserById(id);
    }

    public async Task<IUserModel?> GetByUsername(string username)
    {
      var user = await _userRepository.GetUserByUsername(username);
      return user;
    }

    public async Task<IUserModel?> GetUserById(int user_id)
    {
      var user = await _userRepository.GetUserById(user_id);
      return user;
    }

    public async Task<bool> PatchUser(int id, UserViewModel userToUpdate)
    {
      IUserModel userInDatabase = await GetUserById(id);
      if (userInDatabase == null)
      {
        return false;
      }

      if (_authorizationHelper.IsRefusedToPerformActionOnUser(id, userInDatabase))
      {
        return false;
      }

      if (userInDatabase.Name != userToUpdate.Name)
      {
        userInDatabase.Name = userToUpdate.Name;
      }

      bool isUserUpdated = await _userRepository.UpdateUser(userInDatabase);
      return isUserUpdated;
    }

    public async Task<bool> PatchUserPassword(int id, string currentPassword, string newPassword)
    {
      if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(currentPassword))
      {
        return false;
      }

      IUserModel userInDatabase = await GetUserById(id);
      if (userInDatabase == null)
      {
        return false;
      }

      if (_authorizationHelper.IsRefusedToPerformActionOnUser(id, userInDatabase))
      {
        return false;
      }

      if (!_cryptographyUtil.IsPasswordValid(newPassword, EnumPasswordComplexity.Medium))
      {
        return false;
      }

      if (!_cryptographyUtil.VerifyHash(currentPassword, userInDatabase.Salt, userInDatabase.Hash))
      {
        return false;
      }

      userInDatabase.Salt = _cryptographyUtil.GenerateSalt();
      userInDatabase.Hash = _cryptographyUtil.HashPassword(newPassword, userInDatabase.Salt);

      bool isUserUpdated = await _userRepository.UpdateUser(userInDatabase);
      return isUserUpdated;
    }
  }
}