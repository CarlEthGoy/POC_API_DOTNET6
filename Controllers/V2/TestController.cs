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
    public TestController(IUserRepository repository)
    {
      _repository = repository;
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