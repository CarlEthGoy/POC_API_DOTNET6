using API.Cryptography;
using API.Database;
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
    private readonly IPasswordRepository _passwordRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVaultRepository _vaultRepository;

    public BLLPassword(IPasswordRepository passwordRepository, IVaultRepository vaultRepository, IUserRepository userRepository)
    {
      _passwordRepository = passwordRepository;
      _vaultRepository = vaultRepository;
      _userRepository = userRepository;
    }

    public async Task<int> CreatePassword(IPasswordViewModel passwordViewModelToCreate)
    {
      if (string.IsNullOrWhiteSpace(passwordViewModelToCreate.Application_name))
      {
        throw new Exception("Application_name is required.");
      }

      if (await _vaultRepository.GetVaultById(passwordViewModelToCreate.Vault_id) == null)
      {
        throw new Exception($"The vault with id:{passwordViewModelToCreate.Vault_id} doesn't exist!");
      }

      IPasswordModel passwordModelToCreate = _passwordRepository.GeneratePasswordModelFromPasswordViewModel(passwordViewModelToCreate);
      int createdPasswordId = await _passwordRepository.CreatePassword(passwordModelToCreate);

      bool createdRelationShip = await _passwordRepository.CreateRelationshipMember(passwordViewModelToCreate.Vault_id, createdPasswordId);
      if (!createdRelationShip)
      {
        throw new Exception($"Couln't create the relationship between vault:{passwordViewModelToCreate.Vault_id} and password:{createdPasswordId} !");
      }

      return createdPasswordId;
    }

    public async Task<bool> DeletePasswordById(int password_id)
    {
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

      if (passwordInDatabase.Application_name != passwordToPatch.Application_name)
      {
        passwordInDatabase.Application_name = passwordToPatch.Application_name;
      }

      if (passwordInDatabase.Username != passwordToPatch.Username)
      {
        passwordInDatabase.Username = passwordToPatch.Username;
      }

      if (!string.IsNullOrWhiteSpace(passwordToPatch.Password))
      {
        passwordInDatabase.Encrypted_password = CryptographyUtil.Instance.EncryptasswordForString(passwordToPatch.Password, Enum.EnumPasswordComplexity.Medium);
      }

      bool isPasswordPatched = await _passwordRepository.UpdatePassword(passwordInDatabase);
      return isPasswordPatched;
    }
  }
}