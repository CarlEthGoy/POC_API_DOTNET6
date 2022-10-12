using API.Models.V2;
using Neo4j.Driver;

namespace API.DDL
{
  public interface IUserRepository
  {
    Task<IEnumerable<User>> GetAll();

    Task<User> CreateUser(User userToAdd);
  }

  public class UserRepository : IUserRepository
  {
    private readonly IConfiguration _configuration;
    private bool _disposed = false;
    private readonly IDriver _driver;

    #region Database Actions
    public async Task<User> CreateUser(User userToAdd)
    {
      User createdUser = new();

      var query = @$"CREATE (user:User {{id: '{Guid.NewGuid()}', name: '{userToAdd.name}', hash: '{userToAdd.hash}'}}) RETURN user";

      await using var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration.GetValue<string>("Neo4JSettings:Database")));
      try
      {
        var writeResults = await session.ExecuteWriteAsync(async tx =>
        {
          var result = await tx.RunAsync(query);
          return await result.ToListAsync();
        });

        if (writeResults == null || writeResults.Count == 0 || writeResults.FirstOrDefault() == null) { throw new Exception("Creation failed."); }

        createdUser = new User
        {
          name = writeResults.First()["user"].As<INode>().Properties["name"].As<string>(),
          hash = writeResults.First()["user"].As<INode>().Properties["hash"].As<string>(),
          uuid = new Guid(writeResults.First()["user"].As<INode>().Properties["id"].As<string>()),
        };

      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }

      return createdUser;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
      var lstUser = new List<User>();

      var query = @"MATCH (user:User) RETURN user";

      await using var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration.GetValue<string>("Neo4JSettings:Database")));
      try
      {
        var readResults = await session.ExecuteReadAsync(async tx =>
        {
          var result = await tx.RunAsync(query);
          return await result.ToListAsync();
        });

        foreach (var result in readResults)
        {
          var user = new User
          {
            name = result["user"].As<INode>().Properties["name"].As<string>(),
            hash = result["user"].As<INode>().Properties["hash"].As<string>(),
            uuid = new Guid(result["user"].As<INode>().Properties["id"].As<string>()),
          };
          lstUser.Add(user);
        }
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }

      return lstUser;
    }
    #endregion


    #region Constructeur et Dispose
    public UserRepository(IConfiguration configuration)
    {
      _configuration = configuration;
      string uri = _configuration.GetValue<string>("Neo4JSettings:Connection");
      string user = _configuration.GetValue<string>("Neo4JSettings:User");
      string password = _configuration.GetValue<string>("Neo4JSettings:Password");

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