using API.BLL;
using API.Controllers.V1;
using API.Database;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.Controllers
{
  public class TestAuthController : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_Login_Fail()
    {
      // Arrange
      var mockUserRepository = new Mock<IUserRepository>();
      var mockBllAuthentication = new Mock<IBLLAuthentication>();

      var mockedUserModel = new UserModel
      {
        Salt = new byte[16] { 245, 160, 215, 187, 205, 23, 232, 203, 183, 50, 217, 211, 209, 125, 4, 202 },
        Username = "Carl",
        Hash = new byte[16] { 84, 210, 16, 102, 13, 110, 173, 176, 136, 71, 121, 70, 5, 7, 33, 200 },
        Name = "carl"
      };

      mockBllAuthentication.Setup(x => x.Login("carl", "123").Result)
                     .Returns(false);

      mockUserRepository.Setup(x => x.GetUserByUsername(mockedUserModel.Name).Result)
                        .Returns(mockedUserModel);

      var authController = new AuthController(mockUserRepository.Object, mockBllAuthentication.Object);

      // Act
      var isLoggedIn = authController.Login("Carl", "123").Result;

      // Assert
      Assert.IsTrue(!isLoggedIn);
    }

    [Test]
    public void Test_Login_Success()
    {
      // Arrange
      var mockBllAuthentication = new Mock<IBLLAuthentication>();
      var mockUserRepository = new Mock<IUserRepository>();

      var mockedUserModel = new UserModel
      {
        Salt = new byte[16] { 245, 160, 215, 187, 205, 23, 232, 203, 183, 50, 217, 211, 209, 125, 4, 202 },
        Username = "Carl",
        Hash = new byte[16] { 84, 210, 16, 102, 13, 110, 173, 176, 136, 71, 121, 70, 5, 7, 33, 200 },
        Name = "carl"
      };

      mockBllAuthentication.Setup(x => x.Login("Carl", "123").Result)
                           .Returns(true);

      mockUserRepository.Setup(x => x.GetUserByUsername(mockedUserModel.Username).Result)
                        .Returns(mockedUserModel);

      var authController = new AuthController(mockUserRepository.Object, mockBllAuthentication.Object);

      // Act
      var isLoggedIn = authController.Login("Carl", "123").Result;

      // Assert
      Assert.IsTrue(isLoggedIn);
    }
  }
}
