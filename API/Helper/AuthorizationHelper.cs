using API.Database;
using API.Models.V1;

namespace API.Helper
{
  public interface IAuthorizationHelper
  {
    bool IsRefusedToPerformActionOnUser(int id, IUserModel user);
    Task<bool> IsRefusedToPerformActionOnUser(int id);
  }

  public class AuthorizationHelper : IAuthorizationHelper
  {
    public readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationHelper(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
      _userRepository = userRepository;
      _httpContextAccessor = httpContextAccessor;
    }

    #region UserRepository
    public bool IsRefusedToPerformActionOnUser(int id, IUserModel user)
    {
      string loggedInUsername = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "";
      if (string.IsNullOrWhiteSpace(loggedInUsername))
      {
        return true;
      }

      if (user == null)
      {
        return true;
      }

      if (user.Username == loggedInUsername)
      {
        return false;
      }

      return true;
    }

    public async Task<bool> IsRefusedToPerformActionOnUser(int id)
    {
      string loggedInUsername = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "";
      if (string.IsNullOrWhiteSpace(loggedInUsername))
      {
        return true;
      }

      IUserModel userInDatabase = await _userRepository.GetUserById(id);
      if (userInDatabase == null)
      {
        return true;
      }

      if (userInDatabase.Username == loggedInUsername)
      {
        return false;
      }

      return true;
    }
    #endregion
  }
}
