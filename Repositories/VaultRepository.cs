using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface IVaultRepository
  {
    Task<int?> CreateVault(IVaultModel vaultToAdd);
    Task<bool> DeleteVaultById(int vaultId);
    Task<IVaultModel?> GetVaultById(int id);
    Task<List<IVaultModel>> GetAllVaultForUserId(int userId);
    Task<bool> CreateRelationshipOwner(IVaultModel vaultViewModel);
  }

  public class VaultRepository : IVaultRepository
  {
    private readonly IConfiguration _configuration;
    private bool _disposed = false;
    private readonly IDriver _driver;


    #region Database Actions
    public async Task<List<IVaultModel>> GetAllVaultForUserId(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (vault:Vault)
                     WHERE id(vault) = $id
                     RETURN collect({
                              id: ID(vault),
                              name: vault.name,
                            }) as vault";

          var parameters = new Dictionary<string, object> {
            { "id", id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.ToListAsync<IVaultModel>(record => new VaultModel
          {
            Id = record["id"].As<int>(),
            Name = record["name"].As<string>()
          });
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<IVaultModel?> GetVaultById(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (vault:Vault)
                     WHERE id(vault) = $id
                     RETURN collect({
                              id: ID(vault),
                              name: vault._name,
                            }) as vault";

          var parameters = new Dictionary<string, object> {
            { "id", id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync<IVaultModel?>(record => ToVaultModel(record["vault"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<int?> CreateVault(IVaultModel vaultToAdd)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "CREATE (vault:Vault { name: $name }) RETURN vault";

          var parameters = new Dictionary<string, object> {
            { "name", vaultToAdd.Name},
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => record["vault"].As<INode>().ElementId.As<int>());
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<bool> CreateRelationshipOwner(IVaultModel vaultToAdd)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = @"MATCH (user) WHERE id(user) = $user_id
                        MATCH (vault) WHERE id(vault) = $vault_id
                        MERGE (user)-[:OWNER{date_created:$date_created, date_updated:$date_updated}]->(vault)
                        RETURN *";

          var parameters = new Dictionary<string, object> {
            { "user_id", vaultToAdd.User_id},
            { "vault_id", vaultToAdd.Id},
            { "date_created", DateTime.Now.ToString() },
            { "date_updated", DateTime.Now.ToString() }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return cursor.ConsumeAsync().IsCompleted;
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<bool> DeleteVaultById(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        var result = await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "MATCH (vault:Vault) WHERE id(vault) = $id DETACH DELETE vault RETURN vault";
          var parameters = new Dictionary<string, object> {
            { "id", id},
          };

          var cursor = await transaction.RunAsync(query, parameters);
          var result = await cursor.SingleAsync(record => record["vault"] != null);

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
    #endregion

    #region Cast
    private static VaultModel? ToVaultModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return ToListVaultModel(datas).FirstOrDefault();
    }
    private static List<VaultModel> ToListVaultModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return datas.Select(dictionary => new VaultModel
      {
        Id = dictionary["id"].As<int>(),
        Name = dictionary["name"].As<string>(),
      }).ToList();
    }
    #endregion

    #region Constructeur et Dispose
    public VaultRepository(IConfiguration configuration, IDriver driver)
    {
      _configuration = configuration;
      _driver = driver;
    }

    ~VaultRepository() => Dispose(false);
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