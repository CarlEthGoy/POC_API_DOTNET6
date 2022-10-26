using API.BLL;
using API.Controllers.V1;
using API.Database;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.Controllers
{
  public class TestPasswordController : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CreatePassword_Fail()
    {
      // Arrange
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockBllPassword = new Mock<IBLLPassword>();

      var mockedPasswordViewModel = new PasswordViewModel
      {
        Application_name = "carl"
      };

      mockBllPassword.Setup(x => x.CreatePassword(mockedPasswordViewModel).Result)
                 .Returns(-1);

      var PasswordController = new PasswordController(mockedVaultRepository.Object, mockBllPassword.Object);

      // Act
      var createdPasswordId = PasswordController.CreatePassword(mockedPasswordViewModel).Result;

      // Assert
      Assert.IsTrue(createdPasswordId == -1);
    }

    [Test]
    public void Test_CreatePassword_Success()
    {
      // Arrange
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockBllPassword = new Mock<IBLLPassword>();

      var mockedPasswordViewModel = new PasswordViewModel
      {
        Application_name = "carl"
      };

      mockBllPassword.Setup(x => x.CreatePassword(mockedPasswordViewModel).Result)
                 .Returns(3);

      var PasswordController = new PasswordController(mockedVaultRepository.Object, mockBllPassword.Object);

      // Act
      var createdPasswordId = PasswordController.CreatePassword(mockedPasswordViewModel).Result;

      // Assert
      Assert.IsTrue(createdPasswordId >= 0);
    }

    [Test]
    public void Test_DeletePasswordById_Fail()
    {
      // Arrange
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedBllPassword = new Mock<IBLLPassword>();
      mockedBllPassword.Setup(x => x.DeletePasswordById(1).Result)
                   .Returns(false);

      PasswordController PasswordController = new(mockedVaultRepository.Object, mockedBllPassword.Object);

      // Act
      bool isDeleted = PasswordController.DeletePasswordById(1).Result;

      // Assert
      Assert.IsTrue(!isDeleted);
    }

    [Test]
    public void Test_DeletePasswordById_Success()
    {
      // Arrange
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedBllPassword = new Mock<IBLLPassword>();
      mockedBllPassword.Setup(x => x.DeletePasswordById(1).Result)
                   .Returns(true);

      PasswordController PasswordController = new(mockedVaultRepository.Object, mockedBllPassword.Object);

      // Act
      bool isDeleted = PasswordController.DeletePasswordById(1).Result;

      // Assert
      Assert.IsTrue(isDeleted);
    }

    [Test]
    public void Test_GetPasswordById_Fail()
    {
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedBllPassword = new Mock<IBLLPassword>();
      mockedBllPassword.Setup(x => x.GetPasswordById(1).Result)
                   .Returns(new PasswordModel());

      PasswordController PasswordController = new(mockedVaultRepository.Object, mockedBllPassword.Object);

      // Act
      PasswordModel password = PasswordController.GetPasswordById(1).Result;

      // Assert
      Assert.IsTrue(password != null);
    }

    [Test]
    public void Test_GetPasswordById_Success()
    {
      var mockedVaultRepository = new Mock<IVaultRepository>();
      var mockedBllPassword = new Mock<IBLLPassword>();

      PasswordController PasswordController = new(mockedVaultRepository.Object, mockedBllPassword.Object);

      // Act
      PasswordModel password = PasswordController.GetPasswordById(1).Result;

      // Assert
      Assert.IsTrue(password == null);
    }

    [Test]
    public void Test_PatchPassword_Fail()
    {
      var mockedBllPassword = new Mock<IBLLPassword>();
      var mockedVaultRepository = new Mock<IVaultRepository>();

      var mockedPassword = new PasswordViewModel
      {
        Application_name = "TestPass",
        Password = "",
        Username = "TestCarl",
      };

      mockedBllPassword.Setup(x => x.PatchPassword(1, mockedPassword).Result).Returns(false);

      PasswordController passwordController = new(mockedVaultRepository.Object, mockedBllPassword.Object);

      // Act
      bool isVaultPatchFail = !passwordController.PatchPassword(1, mockedPassword).Result;

      // Assert
      Assert.IsTrue(isVaultPatchFail);
    }

    [Test]
    public void Test_PatchPassword_Success()
    {
      var mockedBllPassword = new Mock<IBLLPassword>();
      var mockedVaultRepository = new Mock<IVaultRepository>();

      var mockedPassword = new PasswordViewModel
      {
        Application_name = "TestPass",
        Password = "",
        Username = "TestCarl",
      };

      mockedBllPassword.Setup(x => x.PatchPassword(1, mockedPassword).Result).Returns(true);

      PasswordController passwordController = new(mockedVaultRepository.Object, mockedBllPassword.Object);

      // Act
      bool isVaultPatchSuccess = passwordController.PatchPassword(1, mockedPassword).Result;

      // Assert
      Assert.IsTrue(isVaultPatchSuccess);
    }
  }
}