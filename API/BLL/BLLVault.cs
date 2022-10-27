using API.Database;
using API.Helper;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLVault
  {
    Task<int> CreateVault(IVaultViewModel vaultViewModel);

    Task<bool> DeleteVaultById(int id);

    Task<IVaultModel> GetVaultById(int id);

    Task<bool> PatchVault(int id, VaultViewModel vaultToPatch);
  }

  public class BLLVault : IBLLVault
  {
    private readonly IUserRepository _userRepository;
    private readonly IVaultRepository _vaultRepository;
    private readonly IAuthorizationHelper _authorizationHelper;

    public BLLVault(IVaultRepository vaultRepository, IUserRepository userRepository, IAuthorizationHelper authorization)
    {
      _vaultRepository = vaultRepository;
      _userRepository = userRepository;
      _authorizationHelper = authorization;
    }

    public async Task<int> CreateVault(IVaultViewModel vaultViewModelToCreate)
    {
      if (string.IsNullOrWhiteSpace(vaultViewModelToCreate.Name))
      {
        throw new Exception("Name is required.");
      }

      var user = await _userRepository.GetUserById(vaultViewModelToCreate.User_id);
      if (user == null)
      {
        throw new Exception($"The user with id : {vaultViewModelToCreate.User_id} doesn't exist!");
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(vaultViewModelToCreate.User_id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(user.Id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      IVaultModel vaultModelToCreate = _vaultRepository.GenerateVaultModelFromVaultViewModel(vaultViewModelToCreate);

      int createdVaultId = await _vaultRepository.CreateVault(vaultModelToCreate);
      vaultModelToCreate.Id = createdVaultId;

      bool isCreatedRelationShipFailed = !await _vaultRepository.CreateRelationshipOwner(vaultModelToCreate.User_id, vaultModelToCreate.Id);
      if (isCreatedRelationShipFailed)
      {
        throw new Exception($"Couln't create the relationship between vault:{createdVaultId} and user:{vaultViewModelToCreate.User_id} !");
      }

      return createdVaultId;
    }

    public async Task<bool> DeleteVaultById(int vault_id)
    {
      IVaultModel vaultInDatabase = await GetVaultById(vault_id);
      if (vaultInDatabase == null)
      {
        return false;
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(vaultInDatabase.User_id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      return await _vaultRepository.DeleteVaultById(vault_id);
    }

    public async Task<IVaultModel> GetVaultById(int vault_id)
    {
      var vault = await _vaultRepository.GetVaultById(vault_id);
      return vault;
    }

    public async Task<bool> PatchVault(int vault_id, VaultViewModel vaultToPatch)
    {
      IVaultModel vaultInDatabase = await GetVaultById(vault_id);
      if (vaultInDatabase == null)
      {
        return false;
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(vaultInDatabase.User_id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      if (vaultInDatabase.Name != vaultToPatch.Name)
      {
        vaultInDatabase.Name = vaultToPatch.Name;
      }

      bool isVaultPatched = await _vaultRepository.UpdateVault(vaultInDatabase);
      return isVaultPatched;
    }
  }
}