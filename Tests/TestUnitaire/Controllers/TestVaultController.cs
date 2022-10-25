using API.BLL;
using API.Controllers.V1;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.Controllers
{
  public class TestVaultController : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CreateVault_Fail()
    {
      // Arrange
      var mockBllVault = new Mock<IBLLVault>();

      var mockedVaultViewModel = new VaultViewModel
      {
        Name = "carl"
      };

      mockBllVault.Setup(x => x.CreateVault(mockedVaultViewModel).Result)
                 .Returns(-1);

      var vaultController = new VaultController(mockBllVault.Object);

      // Act
      var createdVaultId = vaultController.CreateVault(mockedVaultViewModel).Result;

      // Assert
      Assert.IsTrue(createdVaultId == -1);
    }

    [Test]
    public void Test_CreateVault_Success()
    {
      // Arrange
      var mockBllVault = new Mock<IBLLVault>();

      var mockedVaultViewModel = new VaultViewModel
      {
        Name = "carl"
      };

      mockBllVault.Setup(x => x.CreateVault(mockedVaultViewModel).Result)
                 .Returns(3);

      var vaultController = new VaultController(mockBllVault.Object);

      // Act
      var createdVaultId = vaultController.CreateVault(mockedVaultViewModel).Result;

      // Assert
      Assert.IsTrue(createdVaultId >= 0);
    }

    [Test]
    public void Test_DeleteVaultById_Fail()
    {
      var mockedBllVault = new Mock<IBLLVault>();
      mockedBllVault.Setup(x => x.DeleteVaultById(1).Result)
                   .Returns(false);

      VaultController vaultController = new(mockedBllVault.Object);

      // Act
      bool isDeleted = vaultController.DeleteVaultById(1).Result;

      // Assert
      Assert.IsTrue(isDeleted == false);
    }

    [Test]
    public void Test_DeleteVaultById_Success()
    {
      var mockedBllVault = new Mock<IBLLVault>();
      mockedBllVault.Setup(x => x.DeleteVaultById(1).Result)
                   .Returns(true);

      VaultController vaultController = new(mockedBllVault.Object);

      // Act
      bool isDeleted = vaultController.DeleteVaultById(1).Result;

      // Assert
      Assert.IsTrue(isDeleted);
    }

    [Test]
    public void Test_GetVaultById_Fail()
    {
      var mockedBllVault = new Mock<IBLLVault>();
      VaultController vaultController = new(mockedBllVault.Object);

      // Act
      VaultModel vault = vaultController.GetVaultById(1).Result;

      // Assert
      Assert.IsTrue(vault == null);
    }

    [Test]
    public void Test_GetVaultById_Success()
    {
      var mockedBllVault = new Mock<IBLLVault>();
      mockedBllVault.Setup(x => x.GetVaultById(1).Result)
                   .Returns(new VaultModel());

      VaultController vaultController = new(mockedBllVault.Object);

      // Act
      VaultModel vault = vaultController.GetVaultById(1).Result;

      // Assert
      Assert.IsTrue(vault != null);
    }
  }
}
