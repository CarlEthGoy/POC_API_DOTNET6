using API.BLL;
using API.Database;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.BLL
{
  public class TestBLLVault : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CreateVault_Fail()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();

      var mockedVaultViewModel = new VaultViewModel
      {
        Name = "Test",
        User_id = 1,
      };

      var mockedVaultModel = new VaultModel
      {
        Id = -1,
        Name = "Test",
        User_id = 1,
      };

      mockedVaultRepository.Setup(x => x.CreateVault(mockedVaultModel).Result)
                          .Returns(-1);

      mockedVaultRepository.Setup(x => x.GenerateVaultModelFromVaultViewModel(mockedVaultViewModel))
                          .Returns(mockedVaultModel);

      mockedVaultRepository.Setup(x => x.CreateRelationshipOwner(mockedVaultViewModel.User_id, mockedVaultModel.Id).Result)
                           .Returns(true);

      mockedUserRepository.Setup(x => x.GetUserById(mockedVaultModel.User_id).Result)
                          .Returns(new UserModel { });

      var bllVault = new BLLVault(mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var createdVaultId = bllVault.CreateVault(mockedVaultViewModel).Result;

      // Asssert
      Assert.IsTrue(createdVaultId == -1);
    }

    [Test]
    public void Test_CreateVault_Success()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();

      var mockedVaultViewModel = new VaultViewModel
      {
        Name = "Test",
        User_id = 1,
      };

      var mockedVaultModel = new VaultModel
      {
        Id = 1,
        Name = "Test",
        User_id = 1,
      };

      mockedVaultRepository.Setup(x => x.CreateVault(mockedVaultModel).Result)
                          .Returns(1);

      mockedVaultRepository.Setup(x => x.GenerateVaultModelFromVaultViewModel(mockedVaultViewModel))
                          .Returns(mockedVaultModel);

      mockedVaultRepository.Setup(x => x.CreateRelationshipOwner(mockedVaultViewModel.User_id, mockedVaultModel.Id).Result)
                           .Returns(true);

      mockedUserRepository.Setup(x => x.GetUserById(mockedVaultModel.User_id).Result)
                          .Returns(new UserModel { });

      var bllVault = new BLLVault(mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var createdVaultId = bllVault.CreateVault(mockedVaultViewModel).Result;

      // Asssert
      Assert.IsTrue(createdVaultId >= 0);
    }

    [Test]
    public void Test_DeleteVaultById_Fail()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();

      var mockedVaultToDelete = new VaultModel
      {
        Id = 1,
        Name = "Test"
      };

      mockedVaultRepository.Setup(x => x.DeleteVaultById(mockedVaultToDelete.Id).Result)
        .Returns(false);

      var bllVault = new BLLVault(mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var isDeleted = bllVault.DeleteVaultById(mockedVaultToDelete.Id).Result;

      // Asssert
      Assert.IsTrue(isDeleted == false);
    }

    [Test]
    public void Test_DeleteVaultById_Success()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedVaultRepository = new Mock<IVaultRepository>();

      var mockedVaultToDelete = new VaultModel
      {
        Id = 1,
        Name = "Test"
      };

      mockedVaultRepository.Setup(x => x.DeleteVaultById(mockedVaultToDelete.Id).Result)
        .Returns(true);

      var bllVault = new BLLVault(mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var isDeleted = bllVault.DeleteVaultById(mockedVaultToDelete.Id).Result;

      // Asssert
      Assert.IsTrue(isDeleted);
    }

    [Test]
    public void Test_GetVaultById_Fail()
    {
      // Arrange
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedUserRepository = new Mock<IUserRepository>();

      var mockedVaultToGet = new VaultModel
      {
        Name = "Test",
      };

      var bllVault = new BLLVault(mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var vault = bllVault.GetVaultById(mockedVaultToGet.Id).Result;

      // Asssert
      Assert.IsTrue(vault == null);
    }

    [Test]
    public void Test_GetVaultById_Success()
    {
      // Arrange
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedUserRepository = new Mock<IUserRepository>();

      var mockedVaultToGet = new VaultModel
      {
        Name = "Test",
      };

      mockedVaultRepository.Setup(x => x.GetVaultById(mockedVaultToGet.Id).Result)
                          .Returns(mockedVaultToGet);

      var bllVault = new BLLVault(mockedVaultRepository.Object, mockedUserRepository.Object);

      // Act
      var vault = bllVault.GetVaultById(mockedVaultToGet.Id).Result;

      // Asssert
      Assert.IsTrue(vault != null);
    }
  }
}