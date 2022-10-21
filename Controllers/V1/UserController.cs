using API.BLL;
using API.Database;
using API.Models.V1;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
  //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
  //[Authorize]
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiExplorerSettings(GroupName = "v1")]
  public class UserController : ControllerBase
  {
    private readonly IBLLUser _bllUser;
    private readonly IBLLAuthentication _bllAuthentication;

    public UserController(IUserRepository repository, IBLLUser bllUser, IBLLAuthentication bllAuthentication)
    {
      _bllUser = bllUser;
      _bllAuthentication = bllAuthentication;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{user_id}")]
    public async Task<UserModel?> GetUserById(int user_id)
    {
      UserModel? createdUserId = (UserModel?)await _bllUser.GetUserById(user_id);
      return createdUserId;
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<int?> CreateUser(UserViewModel user)
    {
      int? createdUserId = await _bllUser.CreateUser(user);

      return createdUserId;
    }

    [MapToApiVersion("1.0")]
    [HttpDelete("{user_id}")]
    public async Task<bool> DeleteUserById(int user_id)
    {
      bool isDeleted = await _bllUser.DeleteUserById(user_id);
      return isDeleted;
    }
  }
}