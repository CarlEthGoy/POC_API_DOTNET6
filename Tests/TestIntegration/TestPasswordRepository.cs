using API.Cryptography;
using API.Database;
using API.Models.V1;
using Microsoft.Extensions.Configuration;
using Moq;
using Neo4j.Driver;
using Tests.TestUnitaire;

namespace Tests.TestIntegration
{
  public class Test_PasswordRepository : Test
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_CRUDVaultRepository_SUCESS()
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

      UserRepository userRepository = new(mockConfiguration.Object, mockCryptoUtil.Object, driver);
      VaultRepository vaultRepository = new(mockConfiguration.Object, driver);
      PasswordRepository passwordRepository = new(mockConfiguration.Object, driver);

      var mockedUser = new UserModel
      {
        Hash = new byte[16],
        Name = "Test",
        Salt = new byte[16],
        Username = "Test"
      };

      var mockedVault = new VaultModel { Name = "TestVault" };
      var mockedPassword = new PasswordModel { Application_name = "TestVault" };

      // Act CreateUser
      var createdUserId = userRepository.CreateUser(mockedUser).Result;
      mockedVault.User_id = createdUserId;

      // Act: CreateVault
      var createdVaultId = vaultRepository.CreateVault(mockedVault).Result;
      mockedVault.Id = createdVaultId;
      mockedPassword.Vault_id = createdVaultId;

      // Act: CreateRelationshipOwner
      var isCreateRelationshipOwnerSuccess = vaultRepository.CreateRelationshipOwner(mockedVault.User_id, mockedVault.Id).Result;

      // Act: CreatePassword
      var createdPasswordId = passwordRepository.CreatePassword(mockedPassword).Result;
      mockedPassword.Id = createdPasswordId;

      // Act: CreateRelationshipMember
      var isCreateRelationshipMemberSuccess = passwordRepository.CreateRelationshipMember(mockedPassword.Vault_id, mockedPassword.Id).Result;

      // Act: DeletePasswordById
      var isPasswordDeleted = passwordRepository.DeletePasswordById(createdPasswordId).Result;

      // Act: DeleteVaultById
      var isVaultDeleted = vaultRepository.DeleteVaultById(createdVaultId).Result;

      // Act: DeleteUserById
      var isUserDeleted = userRepository.DeleteUserById(createdUserId).Result;

      // Assert
      Assert.IsTrue(createdUserId >= 0);
      Assert.IsTrue(isVaultDeleted);
      Assert.IsTrue(createdVaultId >= 0);

      Assert.IsTrue(createdPasswordId >= 0);
      Assert.IsTrue(isCreateRelationshipOwnerSuccess);
      Assert.IsTrue(isCreateRelationshipMemberSuccess);

      Assert.IsTrue(isUserDeleted);
      Assert.IsTrue(isPasswordDeleted);
    }
  }
}