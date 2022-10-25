using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface IVaultRepository
  {
    Task<bool> CreateRelationshipOwner(int user_id, int vault_id);
    Task<int> CreateVault(IVaultModel vaultToAdd);
    Task<bool> DeleteVaultById(int vaultId);
    IVaultModel GenerateVaultModelFromVaultViewModel(IVaultViewModel vaultViewModel);
    Task<List<IVaultModel>> GetAllVaultForUserId(int user_id);
    Task<IVaultModel> GetVaultById(int vault_id);
  }

  public class VaultRepository : IVaultRepository
  {
    private readonly IConfiguration _configuration;
    private readonly IDriver _driver;
    private bool _disposed = false;
    #region Database Actions
    public async Task<bool> CreateRelationshipOwner(int user_id, int vault_id)
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
            { "user_id", user_id},
            { "vault_id", vault_id},
            { "date_created", DateTime.Now.ToString() },
            { "date_updated", DateTime.Now.ToString() }
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

    public async Task<int> CreateVault(IVaultModel vaultToAdd)
    {
      if (string.IsNullOrWhiteSpace(vaultToAdd.Name))
      {
        return -1;
      }

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
      catch
      {
        return -1;
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<bool> DeleteVaultById(int vault_id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "MATCH (vault:Vault) WHERE id(vault) = $id DETACH DELETE vault RETURN vault";
          var parameters = new Dictionary<string, object> {
            { "id", vault_id},
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

    public IVaultModel GenerateVaultModelFromVaultViewModel(IVaultViewModel vaultViewModel)
    {
      VaultModel vaultModel = new()
      {
        Name = vaultViewModel.Name,
        User_id = vaultViewModel.User_id
      };

      return vaultModel;
    }

    public async Task<List<IVaultModel>> GetAllVaultForUserId(int vault_id)
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
            { "id", vault_id }
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

    public async Task<IVaultModel> GetVaultById(int vault_id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (vault:Vault)
                     WHERE id(vault) = $id
                     RETURN collect({
                              id: id(vault),
                              name: vault.name
                            }) as vault";

          var parameters = new Dictionary<string, object> {
            { "id", vault_id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync<IVaultModel>(record => ToVaultModel(record["vault"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }
    #endregion

    #region Cast
    private static List<VaultModel> ToListVaultModel(IEnumerable<IDictionary<string, object>> datas)
    {
      if (!datas.Any())
      {
        throw new Exception("The vault doesn't exist.");
      }

      return datas.Select(dictionary => new VaultModel
      {
        Id = dictionary["id"].As<int>(),
        Name = dictionary["name"].As<string>(),
      }).ToList();
    }

    private static VaultModel ToVaultModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return ToListVaultModel(datas).First();
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