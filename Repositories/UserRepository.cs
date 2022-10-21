using API.Cryptography;
using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface IUserRepository
  {
    Task<int?> CreateUser(IUserModel userToAdd);
    Task<bool> DeleteUserById(int id);
    Task<IUserModel?> GetUserById(int id);
    Task<IUserModel?> GetUserByUsername(string username);
    Task<bool> IsUsernameAlreadyUsed(string username);
    IUserModel GenerateUserToCreate(IUserViewModel userViewModel);
  }

  public class UserRepository : IUserRepository
  {
    private readonly IConfiguration _configuration;
    private bool _disposed = false;
    private readonly IDriver _driver;
    private readonly ICryptographyUtil _cryptoUtil;


    #region Database Actions
    public async Task<IUserModel?> GetUserById(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (user:User) 
                     WHERE id(user) = $id
                     RETURN collect({name: user.name,
                              id: ID(user),
                              username: user.username,
                              hash: user.hash,
                              salt: user.salt
                            }) as user";

          var parameters = new Dictionary<string, object> {
            { "id", id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => ToUserModel(record["user"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<int?> CreateUser(IUserModel userToAdd)
    {
      if (string.IsNullOrWhiteSpace(userToAdd.Username))
      {
        return null;
      }

      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "CREATE (user:User {username: $username, name: $name, hash: $hash, salt: $salt}) RETURN user";

          var parameters = new Dictionary<string, object> {
            { "username", userToAdd.Username},
            { "name", userToAdd.Name },
            { "hash", userToAdd.Hash },
            { "salt", userToAdd.Salt }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => record["user"].As<INode>().ElementId.As<int>());
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<bool> DeleteUserById(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        var result = await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "MATCH (user:User) WHERE id(user) = $id DETACH DELETE user RETURN user";
          var parameters = new Dictionary<string, object> {
            { "id", id},
          };

          var cursor = await transaction.RunAsync(query, parameters);
          var result = await cursor.SingleAsync(record => record["user"] != null);

          return result;
        });
        return result;
      }
      catch (Exception)
      {
        return false;
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<IUserModel?> GetUserByUsername(string username)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (user:User) 
                     WHERE user.username = $username
                     RETURN collect({
                              name: user.name,
                              id: ID(user),
                              username: user.username,
                              hash: user.hash,
                              salt: user.salt
                            }) as user";

          var parameters = new Dictionary<string, object> {
            { "username", username }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => ToUserModel(record["user"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<bool> IsUsernameAlreadyUsed(string username)
    {
      var user = await GetUserByUsername(username);
      var isAlreadyUsed = user != null;
      return isAlreadyUsed;
    }

    public IUserModel GenerateUserToCreate(IUserViewModel userViewModel)
    {
      byte[] salt = _cryptoUtil.GenerateSalt();
      byte[] hash = _cryptoUtil.HashPassword(userViewModel.Password, salt);

      UserModel userModel = new()
      {
        Username = userViewModel.Username,
        Hash = hash,
        Name = userViewModel.Name,
        Salt = salt,
      };

      return userModel;
    }
    #endregion

    #region Cast
    private static UserModel? ToUserModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return ToListUserModel(datas).FirstOrDefault();
    }
    private static List<UserModel> ToListUserModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return datas.Select(dictionary => new UserModel
      {
        Id = dictionary["id"].As<int>(),
        Username = dictionary["username"].As<string>(),
        Name = dictionary["name"].As<string>(),
        Hash = dictionary["hash"].As<Byte[]>(),
        Salt = dictionary["salt"].As<Byte[]>()
      }).ToList();
    }
    #endregion

    #region Constructeur et Dispose
    public UserRepository(IConfiguration configuration, ICryptographyUtil cryptoUtil, IDriver driver)
    {
      _cryptoUtil = cryptoUtil;
      _configuration = configuration;
      _driver = driver;
    }

    ~UserRepository() => Dispose(false);
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      if (disposing)
      {
        _driver?.Dispose();
      }

      _disposed = true;
    }
    #endregion


  }
}