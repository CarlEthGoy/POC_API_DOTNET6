using API.BLL;
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

    public UserController(IBLLUser bllUser)
    {
      _bllUser = bllUser;
    }

    [MapToApiVersion("1.0")]
    [HttpPatch("{user_id}/changepassword")]
    public async Task<bool> ChangePassword(int user_id, [FromBody] UserViewModel userToPatch)
    {
      bool isPasswordChanged = await _bllUser.PatchUserPassword(user_id, userToPatch.Password);
      return isPasswordChanged;
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<int> CreateUser([FromBody] UserViewModel userToCreate)
    {
      int createdUserId = await _bllUser.CreateUser(userToCreate);
      return createdUserId;
    }

    [MapToApiVersion("1.0")]
    [HttpDelete("{user_id}")]
    public async Task<bool> DeleteUserById(int user_id)
    {
      bool isDeleted = await _bllUser.DeleteUserById(user_id);
      return isDeleted;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{user_id}")]
    public async Task<UserModel> GetUserById(int user_id)
    {
      UserModel user = (UserModel)await _bllUser.GetUserById(user_id);
      return user;
    }

    [MapToApiVersion("1.0")]
    [HttpPatch("{user_id}")]
    public async Task<bool> PatchUser(int user_id, [FromBody] UserViewModel userToPatch)
    {
      bool isUserPatched = await _bllUser.PatchUser(user_id, userToPatch);
      return isUserPatched;
    }
  }
}