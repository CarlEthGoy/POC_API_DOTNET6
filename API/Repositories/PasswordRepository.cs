﻿using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface IPasswordRepository
  {
    Task<int> CreatePassword(IPasswordModel passwordToAdd);

    Task<bool> CreateRelationshipMember(int vault_id, int password_id);

    Task<bool> DeletePasswordById(int password_id);

    IPasswordModel GeneratePasswordModelFromPasswordViewModel(IPasswordViewModel passwordViewModel);

    Task<IPasswordModel> GetPasswordById(int password_id);

    Task<bool> UpdatePassword(IPasswordModel passwordToUpdate);
  }

  public class PasswordRepository : IPasswordRepository
  {
    private readonly IConfiguration _configuration;
    private readonly IDriver _driver;

    #region Database Actions

    public async Task<int> CreatePassword(IPasswordModel passwordToCreate)
    {
      if (string.IsNullOrWhiteSpace(passwordToCreate.Application_name))
      {
        return -1;
      }

      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "CREATE (password:Password { application_name: $application_name, username: $username, encrypted_password: $encrypted_password}) RETURN password";

          var parameters = new Dictionary<string, object> {
            { "application_name", passwordToCreate.Application_name},
            { "username", passwordToCreate.Username},
            { "encrypted_password", passwordToCreate.Encrypted_password},
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => record["password"].As<INode>().ElementId.As<int>());
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

    public async Task<bool> CreateRelationshipMember(int vault_id, int password_id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = @"MATCH (vault) WHERE id(vault) = $vault_id
                        MATCH (password) WHERE id(password) = $password_id
                        MERGE (password)-[:MEMBER{date_created:$date_created, date_updated:$date_updated}]->(vault)
                        RETURN *";

          var parameters = new Dictionary<string, object> {
            { "vault_id", vault_id},
            { "password_id", password_id},
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

    public async Task<bool> DeletePasswordById(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "MATCH (password:Password) WHERE id(password) = $id DETACH DELETE password RETURN password";
          var parameters = new Dictionary<string, object> {
            { "id", id},
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

    public IPasswordModel GeneratePasswordModelFromPasswordViewModel(IPasswordViewModel passwordViewModel)
    {
      PasswordModel passwordModel = new()
      {
        Application_name = passwordViewModel.Application_name,
        Vault_id = passwordViewModel.Vault_id
      };

      return passwordModel;
    }

    public async Task<List<IPasswordModel>> GetAllForUserId(int password_id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (password:Password)-[:MEMBER_OF]->(vault:Vault)-[:IS_OWNER]->(user:User)
                     WHERE id(user) = $id
                     RETURN collect({
                              id: ID(password),
                              application_name: password.application_name,
                              username: user.username,
                              encrypted_password: user.encrypted_password
                            }) as password";

          var parameters = new Dictionary<string, object> {
            { "id", password_id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.ToListAsync<IPasswordModel>(record => new PasswordModel
          {
            Id = record["id"].As<int>(),
            Username = record["username"].As<string>(),
            Application_name = record["username"].As<string>(),
            Encrypted_password = record["username"].As<string>(),
          });
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<IPasswordModel> GetPasswordById(int password_id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (password:Password)
                     WHERE id(password) = $id
                     RETURN collect({
                              id: ID(user),
                              application_name: password.application_name,
                              username: user.username,
                              encrypted_password: user.encrypted_password
                            }) as password";

          var parameters = new Dictionary<string, object> {
            { "id", password_id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync<IPasswordModel>(record => ToPasswordModel(record["password"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<bool> UpdatePassword(IPasswordModel passwordToUpdate)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = @"MATCH (p:Password)
                        WHERE id(p) = $password_id
                        SET p.application_name: $application_name, p.encrypted_password: $encrypted_password, p.username: $username
                        RETURN p";

          var parameters = new Dictionary<string, object> {
            { "password_id", passwordToUpdate.Id },
            { "application_name", passwordToUpdate.Application_name },
            { "encrypted_password", passwordToUpdate.Encrypted_password },
            { "username", passwordToUpdate.Username}
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
    private static List<PasswordModel> ToListPasswordModel(IEnumerable<IDictionary<string, object>> datas)
    {
      if (!datas.Any())
      {
        throw new Exception("The password doesn't exist.");
      }

      return datas.Select(dictionary => new PasswordModel
      {
        Id = dictionary["id"].As<int>(),
        Username = dictionary["username"].As<string>(),
        Application_name = dictionary["username"].As<string>(),
        Encrypted_password = dictionary["username"].As<string>(),
      }).ToList();
    }

    private static PasswordModel ToPasswordModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return ToListPasswordModel(datas).First();
    }

    #endregion Cast

    #region Constructeur et Dispose

    public PasswordRepository(IConfiguration configuration, IDriver driver)
    {
      _configuration = configuration;
      _driver = driver;
    }
    #endregion Constructeur et Dispose
  }
}