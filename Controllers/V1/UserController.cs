using API.BLL;
using API.Cryptography;
using API.Database;
using API.Models.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API.Controllers.V1
{
  [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
  [Authorize]
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiExplorerSettings(GroupName = "v1")]
  public class UserController : ControllerBase
  {
    private IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
      _repository = repository;
    }

    [MapToApiVersion("1.0")]
    [Route("/login")]
    [HttpGet]
    public async Task<bool> Login(string username, string password)
    {
      BLLUser bllUser = new(_repository);
      UserModel user = await bllUser.GetByUsername(username);

      bool isAuthenticated = CryptographyUtil.VerifyHash(password, user.salt, user.hash);

      return isAuthenticated;
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<string> Create(UserViewModel user)
    {
      BLLUser bllUser = new(_repository);
      string createdUserId = await bllUser.CreateUser(user);

      return createdUserId;
    }

    [MapToApiVersion("1.0")]
    [HttpGet]
    public async Task<UserModel> GetById(int id)
    {
      BLLUser bllUser = new(_repository);
      var user = await bllUser.GetById(id);

      return user;
    }
  }
}