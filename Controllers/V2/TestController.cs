using API.BLL;
using API.Cryptography;
using API.Database;
using API.Models.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API.Controllers.V2
{
  [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
  [Authorize]
  [ApiController]
  [ApiVersion("2.0")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiExplorerSettings(GroupName = "v2")]
  public class TestController : ControllerBase
  {
    private IUserRepository _repository;
    private IConfiguration _configuration;

    public TestController(IUserRepository repository, IConfiguration configuration)
    {
      _repository = repository;
      _configuration = configuration;
    }

    [MapToApiVersion("2.0")]
    [Route("/login")]
    [HttpGet]
    public async Task<bool> Login(string username, string password)
    {
      BLLUser bllUser = new(_repository);
      UserModel user = await bllUser.GetByUsername(username);

      bool isAuthenticated = CryptographyUtil.VerifyHash(password, user.salt, user.hash);

      return isAuthenticated;
    }

    [MapToApiVersion("2.0")]
    [HttpGet]
    public async Task<List<UserModel>> GetAll()
    {
      var test = await _repository.GetAll();
      return test.ToList();
    }

    [MapToApiVersion("2.0")]
    [Route("/{id}")]
    [HttpGet]
    public async Task<UserModel> GetById(int id)
    {
      BLLUser bllUser = new(_repository);
      UserModel user = await bllUser.GetById(id);

      return user;
    }

    [MapToApiVersion("2.0")]
    [HttpPost]
    public async Task<string> Create(UserViewModel user)
    {
      BLLUser bllUser = new(_repository);
      string createdUserId = await bllUser.CreateUser(user);

      return createdUserId;
    }
  }
}