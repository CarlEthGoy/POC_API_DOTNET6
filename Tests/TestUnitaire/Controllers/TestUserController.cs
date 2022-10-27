using API.BLL;
using API.Controllers.V1;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.Controllers
{
  public class Test_UserController : Test
  {
    [Test]
    public void Test_ChangePassword_Fail()
    {
      var mockedBllUser = new Mock<IBLLUser>();

      var mockedChangePasswordModel = new ChangePasswordModel
      {
        Current_password = "123!@#ABCabcD$5fds312z",
        New_password = "123!@#ABCabcD$5fds312z"
      };

      mockedBllUser.Setup(x => x.PatchUserPassword(1, mockedChangePasswordModel.Current_password, mockedChangePasswordModel.New_password).Result)
                   .Returns(false);

      UserController userController = new(mockedBllUser.Object);

      // Act
      var isUserPatchedFail = !userController.ChangePassword(1, mockedChangePasswordModel).Result;

      // Assert
      Assert.IsTrue(isUserPatchedFail);
    }

    [Test]
    public void Test_ChangePassword_Success()
    {
      var mockedBllUser = new Mock<IBLLUser>();

      var mockedChangePasswordModel = new ChangePasswordModel
      {
        Current_password = "123!@#ABCabcD$5fds312z",
        New_password = "123!@#ABCabcD$5fds312z"
      };

      mockedBllUser.Setup(x => x.PatchUserPassword(1, mockedChangePasswordModel.Current_password, mockedChangePasswordModel.New_password).Result)
                   .Returns(true);

      UserController userController = new(mockedBllUser.Object);

      // Act
      var isUserPatchedSuccess = userController.ChangePassword(1, mockedChangePasswordModel).Result;

      // Assert
      Assert.IsTrue(isUserPatchedSuccess);
    }

    [Test]
    public void Test_CreateUser_Fail()
    {
      // Arrange
      var mockBllUser = new Mock<IBLLUser>();

      var mockedUserViewModel = new UserViewModel
      {
        Username = "Carl",
        Name = "carl",
        Password = "123!@#abcCa321#$"
      };

      mockBllUser.Setup(x => x.CreateUser(mockedUserViewModel).Result)
                 .Returns(-1);

      var userController = new UserController(mockBllUser.Object);

      // Act
      var createdUserId = userController.CreateUser(mockedUserViewModel).Result;

      // Assert
      Assert.IsTrue(createdUserId == -1);
    }

    [Test]
    public void Test_CreateUser_Success()
    {
      // Arrange
      var mockBllUser = new Mock<IBLLUser>();

      var mockedUserViewModel = new UserViewModel
      {
        Username = "Carl",
        Name = "carl",
        Password = "123!@#abcCa321#$"
      };

      mockBllUser.Setup(x => x.CreateUser(mockedUserViewModel).Result)
                 .Returns(3);

      var userController = new UserController(mockBllUser.Object);

      // Act
      var createdUserId = userController.CreateUser(mockedUserViewModel).Result;

      // Assert
      Assert.IsTrue(createdUserId >= 0);
    }

    [Test]
    public void Test_DeleteUserById_Fail()
    {
      var mockedBllUser = new Mock<IBLLUser>();
      mockedBllUser.Setup(x => x.DeleteUserById(1).Result)
                   .Returns(false);

      UserController userController = new(mockedBllUser.Object);

      // Act
      var isDeleteFail = !userController.DeleteUserById(1).Result;

      // Assert
      Assert.IsTrue(isDeleteFail);
    }

    [Test]
    public void Test_DeleteUserById_Sucess()
    {
      var mockedBllUser = new Mock<IBLLUser>();
      mockedBllUser.Setup(x => x.DeleteUserById(1).Result)
                   .Returns(true);

      UserController userController = new(mockedBllUser.Object);

      // Act
      var isDeleteSuccess = userController.DeleteUserById(1).Result;

      // Assert
      Assert.IsTrue(isDeleteSuccess);
    }

    [Test]
    public void Test_GetUserById_Fail()
    {
      var mockedBllUser = new Mock<IBLLUser>();
      UserController userController = new(mockedBllUser.Object);

      // Act
      var user = userController.GetUserById(1).Result;

      // Assert
      Assert.IsTrue(user == null);
    }

    [Test]
    public void Test_GetUserById_Success()
    {
      var mockedBllUser = new Mock<IBLLUser>();
      mockedBllUser.Setup(x => x.GetUserById(1).Result)
                   .Returns(new UserModel());

      UserController userController = new(mockedBllUser.Object);

      // Act
      var user = userController.GetUserById(1).Result;

      // Assert
      Assert.IsTrue(user != null);
    }

    [Test]
    public void Test_PatchUser_Fail()
    {
      var mockedBllUser = new Mock<IBLLUser>();

      var mockedUser = new UserViewModel
      {
        Name = "Carl",
        Password = "123!@#ABCabcD$5fds312z",
        Username = "nnnyuuu"
      };

      mockedBllUser.Setup(x => x.PatchUser(1, mockedUser).Result)
                   .Returns(false);

      UserController userController = new(mockedBllUser.Object);

      // Act
      var isUserPatchedFail = !userController.PatchUser(1, mockedUser).Result;

      // Assert
      Assert.IsTrue(isUserPatchedFail);
    }

    [Test]
    public void Test_PatchUser_Success()
    {
      var mockedBllUser = new Mock<IBLLUser>();

      var mockedUser = new UserViewModel
      {
        Name = "Carl",
        Password = "123!@#ABCabcD$5fds312z",
        Username = "nnnyuuu"
      };

      mockedBllUser.Setup(x => x.PatchUser(1, mockedUser).Result)
                   .Returns(true);

      UserController userController = new(mockedBllUser.Object);

      // Act
      var isUserPatchedSuccess = userController.PatchUser(1, mockedUser).Result;

      // Assert
      Assert.IsTrue(isUserPatchedSuccess);
    }
  }
}