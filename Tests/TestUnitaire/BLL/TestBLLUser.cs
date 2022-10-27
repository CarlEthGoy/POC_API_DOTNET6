using API.BLL;
using API.Cryptography;
using API.Database;
using API.Enum;
using API.Helper;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.BLL
{
  public class Test_BLLUser : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CreateUser_Failed()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserViewModel = new UserViewModel
      {
        Name = "Test",
        Password = "123!@#abcAbc123123",
        Username = "Test",
      };

      var mockedUserModel = new UserModel
      {
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.CreateUser(mockedUserModel).Result)
                    .Returns(-1);

      mockedUserRepository.Setup(x => x.GenerateUserModelFromUserViewModel(mockedUserViewModel))
                          .Returns(mockedUserModel);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);
      // Act
      var createdUserId = bllUser.CreateUser(mockedUserViewModel).Result;

      // Asssert
      Assert.IsTrue(createdUserId == -1);
    }

    [Test]
    public void Test_CreateUser_Sucess()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserViewModel = new UserViewModel
      {
        Name = "Test",
        Password = "123!@#abcAbc123123",
        Username = "Test",
      };

      var mockedUserModel = new UserModel
      {
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.CreateUser(mockedUserModel).Result)
                          .Returns(1);

      mockedUserRepository.Setup(x => x.GenerateUserModelFromUserViewModel(mockedUserViewModel))
                          .Returns(mockedUserModel);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var createdUserId = bllUser.CreateUser(mockedUserViewModel).Result;

      // Asssert
      Assert.IsTrue(createdUserId >= 0);
    }

    [Test]
    public void Test_DeleteUserById_Failed()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserToDelete = new UserModel
      {
        Id = 1,
        Username = "Test"
      };

      mockedUserRepository.Setup(x => x.DeleteUserById(mockedUserToDelete.Id).Result)
        .Returns(false);

      mockedAuthorizationHelper.Setup(x => x.IsRefusedToPerformActionOnUser(mockedUserToDelete.Id).Result)
                               .Returns(true);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var isDeleted = bllUser.DeleteUserById(mockedUserToDelete.Id).Result;

      // Asssert
      Assert.IsTrue(!isDeleted);
    }

    [Test]
    public void Test_DeleteUserById_Success()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserToDelete = new UserModel
      {
        Id = 1,
        Username = "Test"
      };

      mockedUserRepository.Setup(x => x.DeleteUserById(mockedUserToDelete.Id).Result)
                          .Returns(true);

      mockedAuthorizationHelper.Setup(x => x.IsRefusedToPerformActionOnUser(mockedUserToDelete.Id).Result)
                               .Returns(false);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var isDeleted = bllUser.DeleteUserById(mockedUserToDelete.Id).Result;

      // Asssert
      Assert.IsTrue(isDeleted);
    }

    [Test]
    public void Test_GetByUsername_Failed()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserToGet = new UserModel
      {
        Username = "Test",
        Hash = new byte[16],
        Id = 1,
        Name = "Test",
        Salt = new byte[16]
      };

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var user = bllUser.GetByUsername(mockedUserToGet.Username).Result;

      // Asssert
      Assert.IsTrue(user == null);
    }

    [Test]
    public void Test_GetByUsername_Sucess()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserToGet = new UserModel
      {
        Username = "Test",
        Hash = new byte[16],
        Id = 1,
        Name = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.GetUserByUsername(mockedUserToGet.Username).Result)
                          .Returns(mockedUserToGet);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var user = bllUser.GetByUsername(mockedUserToGet.Username).Result;

      // Asssert
      Assert.IsTrue(user != null);
    }

    [Test]
    public void Test_GetUserById_Failed()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserToGet = new UserModel
      {
        Username = "Test",
        Hash = new byte[16],
        Id = 1,
        Name = "Test",
        Salt = new byte[16]
      };

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var user = bllUser.GetUserById(mockedUserToGet.Id).Result;

      // Asssert
      Assert.IsTrue(user == null);
    }

    [Test]
    public void Test_GetUserById_Sucess()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserToGet = new UserModel
      {
        Username = "Test",
        Hash = new byte[16],
        Id = 1,
        Name = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.GetUserById(mockedUserToGet.Id).Result)
                          .Returns(mockedUserToGet);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var user = bllUser.GetUserById(mockedUserToGet.Id).Result;

      // Asssert
      Assert.IsTrue(user != null);
    }

    [Test]
    public void Test_PatchUser_Failed()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserViewModel = new UserViewModel
      {
        Name = "Test",
        Password = "123!@#abcAbc123123",
        Username = "Test",
      };

      var mockedUserModel = new UserModel
      {
        Id = 1,
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.UpdateUser(mockedUserModel).Result)
                    .Returns(false);

      mockedUserRepository.Setup(x => x.GetUserById(mockedUserModel.Id).Result)
                          .Returns(mockedUserModel);

      mockedAuthorizationHelper.Setup(x => x.IsRefusedToPerformActionOnUser(mockedUserModel.Id).Result)
                               .Returns(true);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var isPatchFail = !bllUser.PatchUser(mockedUserModel.Id, mockedUserViewModel).Result;

      // Asssert
      Assert.IsTrue(isPatchFail);
    }

    [Test]
    public void Test_PatchUser_Sucess()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserViewModel = new UserViewModel
      {
        Name = "Test",
        Password = "123!@#abcAbc123123",
        Username = "Test",
      };

      var mockedUserModel = new UserModel
      {
        Id = 1,
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.UpdateUser(mockedUserModel).Result)
                    .Returns(true);

      mockedUserRepository.Setup(x => x.GetUserById(mockedUserModel.Id).Result)
                          .Returns(mockedUserModel);


      mockedAuthorizationHelper.Setup(x => x.IsRefusedToPerformActionOnUser(mockedUserModel.Id).Result)
                               .Returns(false);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var isPatchSucces = bllUser.PatchUser(mockedUserModel.Id, mockedUserViewModel).Result;

      // Asssert
      Assert.IsTrue(isPatchSucces);
    }

    [Test]
    public void Test_PatchUserPassword_Failed()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserViewModel = new UserViewModel
      {
        Name = "Test",
        Password = "123!@#abcAbc123123",
        Username = "Test",
      };

      var mockedUserModel = new UserModel
      {
        Id = 1,
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.UpdateUser(mockedUserModel).Result)
                    .Returns(false);

      mockedUserRepository.Setup(x => x.GetUserById(mockedUserModel.Id).Result)
                          .Returns(mockedUserModel);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var isPatchFail = !bllUser.PatchUserPassword(mockedUserModel.Id, "123!@#abcAbc123123", mockedUserViewModel.Password).Result;

      // Asssert
      Assert.IsTrue(isPatchFail);
    }

    [Test]
    public void Test_PatchUserPassword_Sucess()
    {
      // Arrange
      var mockedUserRepository = new Mock<IUserRepository>();
      var mockedAuthorizationHelper = new Mock<IAuthorizationHelper>();
      var mockedCryptographyUtil = new Mock<ICryptographyUtil>();

      var mockedUserViewModel = new UserViewModel
      {
        Name = "Test",
        Password = "123!@#abcAbc123123",
        Username = "Test",
      };

      var mockedUserModel = new UserModel
      {
        Id = 1,
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Salt = new byte[16]
      };

      mockedUserRepository.Setup(x => x.UpdateUser(mockedUserModel).Result)
                    .Returns(true);

      mockedUserRepository.Setup(x => x.GetUserById(mockedUserModel.Id).Result)
                          .Returns(mockedUserModel);

      mockedCryptographyUtil.Setup(x => x.VerifyHash(mockedUserViewModel.Password, mockedUserModel.Salt, mockedUserModel.Hash))
                            .Returns(true);

      mockedCryptographyUtil.Setup(x => x.IsPasswordValid(mockedUserViewModel.Password, EnumPasswordComplexity.Medium))
                            .Returns(true);

      var bllUser = new BLLUser(mockedUserRepository.Object, mockedCryptographyUtil.Object, mockedAuthorizationHelper.Object);

      // Act
      var isPatchSucces = bllUser.PatchUserPassword(mockedUserModel.Id, "123!@#abcAbc123123", mockedUserViewModel.Password).Result;

      // Asssert
      Assert.IsTrue(isPatchSucces);
    }
  }
}