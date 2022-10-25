using API.BLL;
using API.Database;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.BLL
{
  public class TestBLLPassword : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CreatePassword_Fail()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedPasswordRepository = new Mock<IPasswordRepository>();

      var mockedPasswordViewModel = new PasswordViewModel
      {
        Application_name = "Test",
        Vault_id = 1,
      };

      var mockedPasswordModel = new PasswordModel
      {
        Id = -1,
        Application_name = "Test",
        Vault_id = 1,
      };

      mockedPasswordRepository.Setup(x => x.CreatePassword(mockedPasswordModel).Result)
                          .Returns(-1);

      mockedPasswordRepository.Setup(x => x.GeneratePasswordModelFromPasswordViewModel(mockedPasswordViewModel))
                          .Returns(mockedPasswordModel);

      mockedPasswordRepository.Setup(x => x.CreateRelationshipMember(mockedPasswordViewModel.Vault_id, mockedPasswordModel.Id).Result)
                           .Returns(true);

      mockedVaultRepository.Setup(x => x.GetVaultById(mockedPasswordModel.Vault_id).Result)
                          .Returns(new VaultModel { });

      var bllPassword = new BLLPassword(mockedPasswordRepository.Object, mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var createdPasswordId = bllPassword.CreatePassword(mockedPasswordViewModel).Result;

      // Asssert
      Assert.IsTrue(createdPasswordId == -1);
    }

    [Test]
    public void Test_CreatePassword_Success()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedPasswordRepository = new Mock<IPasswordRepository>();

      var mockedPasswordViewModel = new PasswordViewModel
      {
        Application_name = "Test",
        Vault_id = 1,
      };

      var mockedPasswordModel = new PasswordModel
      {
        Id = 1,
        Application_name = "Test",
        Vault_id = 1,
      };

      mockedPasswordRepository.Setup(x => x.CreatePassword(mockedPasswordModel).Result)
                          .Returns(1);

      mockedPasswordRepository.Setup(x => x.GeneratePasswordModelFromPasswordViewModel(mockedPasswordViewModel))
                          .Returns(mockedPasswordModel);

      mockedPasswordRepository.Setup(x => x.CreateRelationshipMember(mockedPasswordViewModel.Vault_id, mockedPasswordModel.Id).Result)
                           .Returns(true);

      mockedVaultRepository.Setup(x => x.GetVaultById(mockedPasswordModel.Vault_id).Result)
                          .Returns(new VaultModel { });

      var bllPassword = new BLLPassword(mockedPasswordRepository.Object, mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var createdPasswordId = bllPassword.CreatePassword(mockedPasswordViewModel).Result;

      // Asssert
      Assert.IsTrue(createdPasswordId >= 0);
    }

    [Test]
    public void Test_DeletePasswordById_Fail()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedPasswordRepository = new Mock<IPasswordRepository>();

      var mockedPasswordToDelete = new PasswordModel
      {
        Id = 1,
        Application_name = "Test"
      };

      mockedPasswordRepository.Setup(x => x.DeletePasswordById(mockedPasswordToDelete.Id).Result)
        .Returns(false);

      var bllPassword = new BLLPassword(mockedPasswordRepository.Object, mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var isDeleted = bllPassword.DeletePasswordById(mockedPasswordToDelete.Id).Result;

      // Asssert
      Assert.IsTrue(isDeleted == false);
    }

    [Test]
    public void Test_DeletePasswordById_Success()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedPasswordRepository = new Mock<IPasswordRepository>();

      var mockedPasswordToDelete = new PasswordModel
      {
        Id = 1,
        Application_name = "Test"
      };

      mockedPasswordRepository.Setup(x => x.DeletePasswordById(mockedPasswordToDelete.Id).Result)
        .Returns(true);

      var bllPassword = new BLLPassword(mockedPasswordRepository.Object, mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var isDeleted = bllPassword.DeletePasswordById(mockedPasswordToDelete.Id).Result;

      // Asssert
      Assert.IsTrue(isDeleted);
    }

    [Test]
    public void Test_GetPasswordById_Fail()
    {
      // Arrange
      var mockedPasswordRepository = new Mock<IPasswordRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedUserRepository = new Mock<IUserRepository>();

      var mockedPasswordToGet = new PasswordModel
      {
        Application_name = "Test",
      };

      var bllPassword = new BLLPassword(mockedPasswordRepository.Object, mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var Password = bllPassword.GetPasswordById(mockedPasswordToGet.Id).Result;

      // Asssert
      Assert.IsTrue(Password == null);
    }

    [Test]
    public void Test_GetPasswordById_Success()
    {
      // Arrange
      var mockedPasswordRepository = new Mock<IPasswordRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedUserRepository = new Mock<IUserRepository>();

      var mockedPasswordToGet = new PasswordModel
      {
        Application_name = "Test",
      };

      mockedPasswordRepository.Setup(x => x.GetPasswordById(mockedPasswordToGet.Id).Result)
                          .Returns(mockedPasswordToGet);

      var bllPassword = new BLLPassword(mockedPasswordRepository.Object, mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var Password = bllPassword.GetPasswordById(mockedPasswordToGet.Id).Result;

      // Asssert
      Assert.IsTrue(Password != null);
    }
  }
}
