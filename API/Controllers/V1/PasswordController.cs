using API.BLL;
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
  public class PasswordController : ControllerBase
  {
#pragma warning disable IDE0052 // Remove unread private members because it is used for TESTS
    private readonly IBLLPassword _bllPassword;
    private readonly IVaultRepository _vaultRepository;
#pragma warning restore IDE0052 // Remove unread private membersbecause it is used for TESTS

    public PasswordController(IVaultRepository vaultRepository, IBLLPassword bllPassword)
    {
      _bllPassword = bllPassword;
      _vaultRepository = vaultRepository;
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<int> CreatePassword([FromBody] PasswordViewModel passwordToCreate)
    {
      int createdPasswordId = await _bllPassword.CreatePassword(passwordToCreate);
      return createdPasswordId;
    }

    [MapToApiVersion("1.0")]
    [HttpDelete("{password_id}")]
    public async Task<bool> DeletePasswordById(int password_id)
    {
      bool isDeleted = await _bllPassword.DeletePasswordById(password_id);
      return isDeleted;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{password_id}")]
    public async Task<PasswordModel> GetPasswordById(int password_id)
    {
      PasswordModel password = (PasswordModel)await _bllPassword.GetPasswordById(password_id);
      return password;
    }

    [MapToApiVersion("1.0")]
    [HttpPatch("{user_id}")]
    public async Task<bool> PatchPassword(int user_id, [FromBody] PasswordViewModel passwordToPatch)
    {
      bool isPasswordPatched = await _bllPassword.PatchPassword(user_id, passwordToPatch);
      return isPasswordPatched;
    }
  }
}