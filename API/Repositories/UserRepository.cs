using API.Cryptography;
using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface IUserRepository
  {
    Task<int> CreateUser(IUserModel userToCreate);

    Task<bool> DeleteUserById(int id);

    IUserModel GenerateUserModelFromUserViewModel(IUserViewModel userViewModel);

    Task<IUserModel> GetUserById(int id);

    Task<IUserModel> GetUserByUsername(string username);

    Task<bool> IsUsernameAlreadyUsed(string username);

    Task<bool> UpdateUser(IUserModel userToUpdate);
  }

  public class UserRepository : IUserRepository
  {
    private readonly IConfiguration _configuration;
    private readonly ICryptographyUtil _cryptoUtil;
    private readonly IDriver _driver;
    private bool _disposed = false;

    #region Database Actions

    public async Task<int> CreateUser(IUserModel userToCreate)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "CREATE (user:User {username: $username, name: $name, hash: $hash, salt: $salt}) RETURN user";

          var parameters = new Dictionary<string, object> {
            { "username", userToCreate.Username},
            { "name", userToCreate.Name },
            { "hash", userToCreate.Hash },
            { "salt", userToCreate.Salt }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => record["user"].As<INode>().ElementId.As<int>());
        });
      }
      catch
      {
        return -1;
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
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "MATCH (user:User) WHERE id(user) = $id DETACH DELETE user RETURN user";
          var parameters = new Dictionary<string, object> {
            { "id", id},
          };

          var cursor = await transaction.RunAsync(query, parameters);
          var result = await cursor.SingleAsync(record => record["user"] != null);

          return result;
        });
      }
      catch
      {
        return false;
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public IUserModel GenerateUserModelFromUserViewModel(IUserViewModel userViewModel)
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

    public async Task<IUserModel> GetUserById(int user_id)
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
            { "id", user_id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync<IUserModel>(record => ToUserModel(record["user"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<IUserModel> GetUserByUsername(string username)
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
      IUserModel user = await GetUserByUsername(username);
      bool isAlreadyUsed = user != null;
      return isAlreadyUsed;
    }

    public async Task<bool> UpdateUser(IUserModel userToUpdate)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = @"MATCH (u:User)
                        WHERE id(u) = $user_id
                        SET u.name = $name, u.hash = $hash, u.salt = $salt
                        RETURN u";

          var parameters = new Dictionary<string, object> {
            { "user_id", userToUpdate.Id },
            { "name", userToUpdate.Name },
            { "hash", userToUpdate.Hash },
            { "salt", userToUpdate.Salt }
          };

          var cursor = await transaction.RunAsync(query, parameters);
          var result = await cursor.FetchAsync();

          return result;
        });
      }
      catch
      {
        return false;
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    #endregion Database Actions

    #region Cast

    private static List<UserModel> ToListUserModel(IEnumerable<IDictionary<string, object>> datas)
    {
      if (!datas.Any())
      {
        throw new Exception("The user doesn't exist.");
      }

      return datas.Select(dictionary => new UserModel
      {
        Id = dictionary["id"].As<int>(),
        Username = dictionary["username"].As<string>(),
        Name = dictionary["name"].As<string>(),
        Hash = dictionary["hash"].As<Byte[]>(),
        Salt = dictionary["salt"].As<Byte[]>()
      }).ToList();
    }

    private static UserModel ToUserModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return ToListUserModel(datas).First();
    }

    #endregion Cast

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

    #endregion Constructeur et Dispose
  }
}