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
  public class PasswordController : ControllerBase
  {
    private readonly IBLLPassword _bllPassword;

    public PasswordController(IVaultRepository repository, IBLLPassword bllPassword)
    {
      _bllPassword = bllPassword;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{password_id}")]
    public async Task<VaultModel> GetPasswordById(int password_id)
    {
      VaultModel createdVault = (VaultModel)await _bllPassword.GetPasswordById(password_id);
      return createdVault;
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<int> CreatePassword(PasswordViewModel passwordToCreate)
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
  }
}
