using API.Database;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLVault
  {
    Task<IVaultModel?> GetVaultById(int id);
    Task<int?> CreateVault(IVaultViewModel vaultViewModel);
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

    public async Task<IVaultModel?> GetVaultById(int id)
    {
      var vault = await _vaultRepository.GetVaultById(id);
      return vault;
    }

    public async Task<int?> CreateVault(IVaultViewModel vaultViewModel)
    {
      VaultModel vaultModel = new()
      {
        Name = vaultViewModel.Name,
        User_id = vaultViewModel.User_id
      };

      // Valider que le username existe!
      if (await _userRepository.GetUserById(vaultViewModel.User_id) == null)
      {
        throw new Exception($"The user with id : {vaultViewModel.User_id} doesn't exist!");
      }

      int? createdVaultId = await _vaultRepository.CreateVault(vaultModel);
      vaultModel.Id = createdVaultId.Value;

      bool createdRelationShip = await _vaultRepository.CreateRelationshipOwner(vaultModel);

      return createdVaultId;
    }

    public async Task<bool> DeleteVaultById(int id)
    {
      return await _vaultRepository.DeleteVaultById(id);
    }
  }
}
