using API.Database;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLVault
  {
    Task<IVaultModel> GetVaultById(int id);
    Task<int> CreateVault(IVaultViewModel vaultViewModel);
    Task<bool> DeleteVaultById(int id);
  }

  public class BLLVault : IBLLVault
  {
    private readonly IVaultRepository _vaultRepository;
    private readonly IUserRepository _userRepository;

    public BLLVault(IVaultRepository vaultRepository, IUserRepository userRepository)
    {
      _vaultRepository = vaultRepository;
      _userRepository = userRepository;
    }

    public async Task<IVaultModel> GetVaultById(int vault_id)
    {
      var vault = await _vaultRepository.GetVaultById(vault_id);
      return vault;
    }

    public async Task<int> CreateVault(IVaultViewModel vaultViewModelToCreate)
    {
      if (string.IsNullOrWhiteSpace(vaultViewModelToCreate.Name))
      {
        throw new Exception("Name is required.");
      }

      if (await _userRepository.GetUserById(vaultViewModelToCreate.User_id) == null)
      {
        throw new Exception($"The user with id : {vaultViewModelToCreate.User_id} doesn't exist!");
      }

      IVaultModel vaultModelToCreate = _vaultRepository.GenerateVaultModelFromVaultViewModel(vaultViewModelToCreate);

      int createdVaultId = await _vaultRepository.CreateVault(vaultModelToCreate);
      vaultModelToCreate.Id = createdVaultId;

      bool createdRelationShip = await _vaultRepository.CreateRelationshipOwner(vaultModelToCreate.User_id, vaultModelToCreate.Id);
      if (!createdRelationShip)
      {
        throw new Exception($"Couln't create the relationship between vault:{createdVaultId} and user:{vaultViewModelToCreate.User_id} !");
      }

      return createdVaultId;
    }

    public async Task<bool> DeleteVaultById(int vault_id)
    {
      return await _vaultRepository.DeleteVaultById(vault_id);
    }
  }
}
