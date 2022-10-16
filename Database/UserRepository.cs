using API.Core;
using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface IUserRepository
  {
    Task<IEnumerable<UserModel>> GetAll();

    Task<string> CreateUser(UserModel userToAdd);

    Task<UserModel> GetById(int id);

    Task<UserModel> GetByUsername(string username);

    Task<bool> UsernameAlreadyUsed(string username);
  }

  public class UserRepository : MustInitialize<IConfiguration>, IUserRepository
  {
    private readonly IConfiguration _configuration;
    private bool _disposed = false;
    private readonly IDriver _driver;

    #region Database Actions
    public async Task<string> CreateUser(UserModel userToAdd)
    {
      try
      {
        string createdUserId = "";

        //Valider que le username n'existe pas deja!
        if (await UsernameAlreadyUsed(userToAdd.username))
        {
          throw new Exception($"This username is already in use : {userToAdd.username}");
        }

        var query = "CREATE (user:User {username: $username, name: $name, hash: $hash, salt: $salt}) RETURN user";
        var parameters = new Dictionary<string, object> {
        { "username", userToAdd.username},
        { "name", userToAdd.name },
        { "hash", userToAdd.hash },
        { "salt", userToAdd.salt }
      };

        List<IRecord> writeResults = await Neo4J.RunExecuteWriteAsync(_driver, _configuration, query, parameters);

        createdUserId = writeResults.First()["user"].As<INode>().ElementId.As<string>();

        return createdUserId;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<IEnumerable<UserModel>> GetAll()
    {
      try
      {
        var lstUser = new List<UserModel>();

        var query = @"MATCH (user:User) RETURN user";

        List<IRecord> readResults = await Neo4J.RunExecuteReadAsync(_driver, _configuration, query);

        foreach (var result in readResults)
        {
          var user = new UserModel
          {
            username = result["user"].As<INode>().Properties["username"].As<string>(),
            name = result["user"].As<INode>().Properties["name"].As<string>(),
            hash = result["user"].As<INode>().Properties["hash"].As<byte[]>(),
            salt = result["user"].As<INode>().Properties["salt"].As<byte[]>(),
          };
          lstUser.Add(user);
        }

        return lstUser;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<UserModel> GetById(int id)
    {
      try
      {
        UserModel? user = null;

        var query = @$"MATCH (user:User) 
                     WHERE id(user) = $id
                     RETURN user";

        var parameters = new Dictionary<string, object> {
          { "id", id}
        };


        List<IRecord> readResults = await Neo4J.RunExecuteReadAsync(_driver, _configuration, query, parameters);

        if (readResults == null || readResults.Count == 0) { throw new Exception($"There is no user with the id {id}"); }

        user = new UserModel
        {
          username = readResults.First()["user"].As<INode>().Properties["username"].As<string>(),
          name = readResults.First()["user"].As<INode>().Properties["name"].As<string>(),
          hash = readResults.First()["user"].As<INode>().Properties["hash"].As<byte[]>(),
          salt = readResults.First()["user"].As<INode>().Properties["salt"].As<byte[]>(),
        };
        return user;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<UserModel> GetByUsername(string username)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(username)) { throw new Exception($"username field is required"); }

        UserModel? user = null;

        var query = @$"MATCH (user:User) 
                     WHERE user.username = $username
                     RETURN user";
        var parameters = new Dictionary<string, object> {
          { "username", username}
        };

        List<IRecord> readResults = await Neo4J.RunExecuteReadAsync(_driver, _configuration, query, parameters);

        if (readResults == null || readResults.Count == 0) { throw new Exception($"There is no user with the username {username}"); }

        user = new UserModel
        {
          username = readResults.First()["user"].As<INode>().Properties["username"].As<string>(),
          name = readResults.First()["user"].As<INode>().Properties["name"].As<string>(),
          hash = readResults.First()["user"].As<INode>().Properties["hash"].As<byte[]>(),
          salt = readResults.First()["user"].As<INode>().Properties["salt"].As<byte[]>(),
        };

        return user;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> UsernameAlreadyUsed(string username)
    {

      try
      {
        if (string.IsNullOrWhiteSpace(username)) { throw new Exception($"username field is required"); }

        var query = @$"MATCH (user:User) 
                     WHERE user.username = $username
                     RETURN user";
        var parameters = new Dictionary<string, object> {
          { "username", username}
        };

        List<IRecord> readResults = await Neo4J.RunExecuteReadAsync(_driver, _configuration, query, parameters);

        return readResults != null && readResults.Count > 0;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }
    #endregion


    #region Constructeur et Dispose
    public UserRepository(IConfiguration configuration) : base(configuration)
    {
      _configuration = configuration;
      string uri = _configuration["Neo4JSettings:Connection"];
      string user = _configuration["Neo4JSettings:User"];
      string password = _configuration["Neo4JSettings:Password"];

      _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
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