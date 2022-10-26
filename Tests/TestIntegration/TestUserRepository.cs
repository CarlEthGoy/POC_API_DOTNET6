using API.Cryptography;
using API.Database;
using API.Models.V1;
using Microsoft.Extensions.Configuration;
using Moq;
using Neo4j.Driver;
using Tests.TestUnitaire;

namespace Tests.TestIntegration
{
  public class Test_UserRepository : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CRUDUserRepository_SUCESS()
    {
      // Arrange
      var mockConfiguration = new Mock<IConfiguration>();
      var mockCryptoUtil = new Mock<ICryptographyUtil>();

      var config = InitConfiguration();
      var connection = config["Neo4JSettings:Connection"];
      var database = config["Neo4JSettings:Database"];
      var user = config["Neo4JSettings:User"];
      var password = config["Neo4JSettings:Password"];

      var driver = GraphDatabase.Driver(connection, AuthTokens.Basic(user, password));
      mockConfiguration.Setup(x => x["Neo4JSettings:Database"])
                       .Returns(database);

      mockConfiguration.Setup(x => x["Neo4JSettings:Connection"])
                       .Returns(connection);

      mockConfiguration.Setup(x => x["Neo4JSettings:User"])
                       .Returns(user);

      mockConfiguration.Setup(x => x["Neo4JSettings:Password"])
                       .Returns(password);

      var mockedUser = new UserModel
      {
        Hash = new byte[16],
        Name = "Test",
        Salt = new byte[16],
        Username = "Test"
      };

      UserRepository userRepository = new(mockConfiguration.Object, mockCryptoUtil.Object, driver);

      // Act: CreateUser
      var createdUserId = userRepository.CreateUser(mockedUser).Result;

      // Act: GetUserById
      var userById = userRepository.GetUserById(createdUserId).Result;

      // Act: GetUserByUsername
      var userByUsername = userRepository.GetUserByUsername(mockedUser.Username).Result;

      // Act: DeleteUserById
      var isUserDeleted = userRepository.DeleteUserById(createdUserId).Result;

      // Assert
      Assert.IsTrue(createdUserId >= 0);
      Assert.IsTrue(userById != null);
      Assert.IsTrue(userByUsername != null);
      Assert.IsTrue(isUserDeleted);
    }
  }
}