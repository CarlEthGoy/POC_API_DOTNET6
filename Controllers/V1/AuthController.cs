using API.BLL;
using API.Database;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiExplorerSettings(GroupName = "v1")]
  public class AuthController : ControllerBase
  {
    private readonly IBLLAuthentication _bllAuthentication;

    public AuthController(IUserRepository repository, IBLLAuthentication bllAuthentication)
    {
      _bllAuthentication = bllAuthentication;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("Login")]
    public async Task<bool> Login(string username, string password)
    {
      bool isAuthorize = await _bllAuthentication.Login(username, password);
      return isAuthorize;
    }

  }
}
