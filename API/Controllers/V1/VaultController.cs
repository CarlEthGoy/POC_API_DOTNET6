﻿using API.BLL;
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
  public class VaultController : ControllerBase
  {
    private readonly IBLLVault _bllVault;

    public VaultController(IBLLVault bllVault)
    {
      _bllVault = bllVault;
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<int> CreateVault([FromBody] VaultViewModel vaultToCreate)
    {
      int createdVaultId = await _bllVault.CreateVault(vaultToCreate);
      return createdVaultId;
    }

    [MapToApiVersion("1.0")]
    [HttpDelete("{vault_id}")]
    public async Task<bool> DeleteVaultById(int vault_id)
    {
      bool isDeleted = await _bllVault.DeleteVaultById(vault_id);
      return isDeleted;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{vault_id}")]
    public async Task<VaultModel> GetVaultById(int vault_id)
    {
      var vault = (VaultModel)await _bllVault.GetVaultById(vault_id);
      return vault;
    }

    [MapToApiVersion("1.0")]
    [HttpPatch("{user_id}")]
    public async Task<bool> PatchVault(int user_id, [FromBody] VaultViewModel vaultToPatch)
    {
      bool isVaultPatched = await _bllVault.PatchVault(user_id, vaultToPatch);
      return isVaultPatched;
    }
  }
}