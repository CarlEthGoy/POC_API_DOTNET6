using API.Cryptography;
using API.Database;
using API.Helper;
using API.Models.V1;

namespace API.BLL
{
  public interface IBLLPassword
  {
    Task<int> CreatePassword(IPasswordViewModel passwordToCreate);

    Task<bool> DeletePasswordById(int password_id);

    Task<IPasswordModel> GetPasswordById(int password_id);

    Task<bool> PatchPassword(int id, PasswordViewModel passwordToPatch);
  }

  public class BLLPassword : IBLLPassword
  {
    private readonly IUserRepository _userRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IVaultRepository _vaultRepository;
    private readonly IAuthorizationHelper _authorizationHelper;

    public BLLPassword(IPasswordRepository passwordRepository, IVaultRepository vaultRepository, IUserRepository userRepository, IAuthorizationHelper authorizationHelper)
    {
      _passwordRepository = passwordRepository;
      _vaultRepository = vaultRepository;
      _userRepository = userRepository;
      _authorizationHelper = authorizationHelper;
    }

    public async Task<int> CreatePassword(IPasswordViewModel passwordViewModelToCreate)
    {
      if (string.IsNullOrWhiteSpace(passwordViewModelToCreate.Application_name))
      {
        throw new Exception("Application_name is required.");
      }

      IVaultModel vaultInDatabase = await _vaultRepository.GetVaultById(passwordViewModelToCreate.Vault_id);
      if (vaultInDatabase == null)
      {
        throw new Exception($"The vault with id:{passwordViewModelToCreate.Vault_id} doesn't exist!");
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(vaultInDatabase.User_id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      IPasswordModel passwordModelToCreate = _passwordRepository.GeneratePasswordModelFromPasswordViewModel(passwordViewModelToCreate);
      int createdPasswordId = await _passwordRepository.CreatePassword(passwordModelToCreate);

      bool isCreatedRelationShipFailed = !await _passwordRepository.CreateRelationshipMember(passwordViewModelToCreate.Vault_id, createdPasswordId);
      if (isCreatedRelationShipFailed)
      {
        throw new Exception($"Couln't create the relationship between vault:{passwordViewModelToCreate.Vault_id} and password:{createdPasswordId} !");
      }

      return createdPasswordId;
    }

    public async Task<bool> DeletePasswordById(int password_id)
    {
      IPasswordModel passwordInDatabase = await _passwordRepository.GetPasswordById(password_id);
      if (passwordInDatabase == null)
      {
        throw new Exception($"The password with the id: {password_id} doesn't exist!");
      }

      IVaultModel vaultInDatabase = await _vaultRepository.GetVaultById(passwordInDatabase.Vault_id);
      if (vaultInDatabase == null)
      {
        throw new Exception($"The vault doesn't exist!");
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(vaultInDatabase.User_id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      return await _passwordRepository.DeletePasswordById(password_id);
    }

    public async Task<IPasswordModel> GetPasswordById(int password_id)
    {
      IPasswordModel password = await _passwordRepository.GetPasswordById(password_id);
      return password;
    }

    public async Task<bool> PatchPassword(int id, PasswordViewModel passwordToPatch)
    {
      IPasswordModel passwordInDatabase = await GetPasswordById(id);
      if (passwordInDatabase == null)
      {
        return false;
      }

      IVaultModel vaultInDatabase = await _vaultRepository.GetVaultById(passwordInDatabase.Vault_id);
      if (vaultInDatabase == null)
      {
        throw new Exception($"The vault doesn't exist!");
      }

      if (await _authorizationHelper.IsRefusedToPerformActionOnUser(vaultInDatabase.User_id))
      {
        throw new Exception($"You don't have the authorization!");
      }

      if (passwordInDatabase.Application_name != passwordToPatch.Application_name)
      {
        passwordInDatabase.Application_name = passwordToPatch.Application_name;
      }

      if (passwordInDatabase.Username != passwordToPatch.Username)
      {
        passwordInDatabase.Username = passwordToPatch.Username;
      }

      if (string.IsNullOrWhiteSpace(passwordToPatch.Password))
      {
        passwordInDatabase.Encrypted_password = CryptographyUtil.Instance.EncryptasswordForString(passwordToPatch.Password, Enum.EnumPasswordComplexity.Medium);
      }

      bool isPasswordPatched = await _passwordRepository.UpdatePassword(passwordInDatabase);
      return isPasswordPatched;
    }
  }
}