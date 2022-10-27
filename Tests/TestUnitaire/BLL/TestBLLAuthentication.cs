using API.BLL;
using API.Cryptography;
using API.Database;
using API.Models.V1;
using Moq;

namespace Tests.TestUnitaire.BLL
{
  public class TestBLLAuthentication : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_Login_Failed()
    {
      // Arrange
      Mock<IUserRepository> mockUserRepository = new();
      Mock<ICryptographyUtil> mockCryptoUtil = new();

      UserViewModel mockedUserViewModel = new()
      {
        Name = "Test",
        Password = "123!@#abcAbc",
        Username = "Test",
      };

      UserModel mockedUserModel = new()
      {
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Id = 0,
        Salt = new byte[16]
      };

      mockUserRepository.Setup(x => x.GetUserByUsername(mockedUserModel.Username).Result)
                 .Returns(mockedUserModel);

      mockCryptoUtil.Setup(x => x.VerifyHash(mockedUserViewModel.Password, mockedUserModel.Salt, mockedUserModel.Hash))
                    .Returns(false);

      BLLAuthentication bllAuthentication = new(mockUserRepository.Object, mockCryptoUtil.Object);

      // Act
      bool isLoggedIn = bllAuthentication.Login(mockedUserViewModel.Username, mockedUserViewModel.Password).Result;

      // Asssert
      Assert.That(isLoggedIn, Is.False);
    }

    [Test]
    public void Test_Login_Success()
    {
      // Arrange
      Mock<IUserRepository> mockUserRepository = new();
      Mock<ICryptographyUtil> mockCryptoUtil = new();

      UserViewModel mockedUserViewModel = new()
      {
        Name = "Test",
        Password = "123!@#abcAbc",
        Username = "Test",
      };

      UserModel mockedUserModel = new()
      {
        Name = "Test",
        Hash = new byte[16],
        Username = "Test",
        Id = 0,
        Salt = new byte[16]
      };

      mockUserRepository.Setup(x => x.GetUserByUsername(mockedUserModel.Username).Result)
                 .Returns(mockedUserModel);

      mockCryptoUtil.Setup(x => x.VerifyHash(mockedUserViewModel.Password, mockedUserModel.Salt, mockedUserModel.Hash))
                    .Returns(true);

      BLLAuthentication bllAuthentication = new(mockUserRepository.Object, mockCryptoUtil.Object);

      // Act
      bool isLoggedIn = bllAuthentication.Login(mockedUserViewModel.Username, mockedUserViewModel.Password).Result;

      // Asssert
      Assert.That(isLoggedIn, Is.True);
    }
  }
}