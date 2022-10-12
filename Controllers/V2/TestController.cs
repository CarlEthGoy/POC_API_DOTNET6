using API.Cryptography;
using API.DDL;
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
    public bool Login(string hashString, string password)
    {
      CryptographyUtil cryptographyUtils = new(_configuration);
      bool isAuthenticated = cryptographyUtils.VerifyHash(hashString, password);
      return isAuthenticated;
    }

    [MapToApiVersion("2.0")]
    [Route("/create-hash")]
    [HttpPost]
    public string CreateHash(string password)
    {
      CryptographyUtil cryptographyUtils = new(_configuration);
      var hashedPassword = cryptographyUtils.HashPassword(password);
      return hashedPassword;
    }


    [MapToApiVersion("2.0")]
    [HttpGet]
    public List<User> GetAll()
    {
      var test = _repository.GetAll();
      return test.Result.ToList();
    }

    [MapToApiVersion("2.0")]
    [HttpPost]
    public User Create(User user)
    {
      var uuidCreated = _repository.CreateUser(user);
      return uuidCreated.Result;
    }
  }
}