using Neo4j.Driver;

namespace API.Database
{
  public static class Neo4J
  {
    public static async Task<List<IRecord>> RunExecuteWriteAsync(IDriver _driver, IConfiguration _configuration, string query, IDictionary<string, object> parameters)
    {
      try
      {
        await using var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration.GetValue<string>("Neo4JSettings:Database")));
        var writeResults = await session.ExecuteWriteAsync(async tx =>
        {
          var result = await tx.RunAsync(query, parameters);
          return await result.ToListAsync();
        });

        if (writeResults == null || writeResults.Count == 0 || writeResults.FirstOrDefault() == null) { throw new Exception("Creation failed."); }

        return writeResults;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async static Task<List<IRecord>> RunExecuteReadAsync(IDriver _driver, IConfiguration _configuration, string query)
    {
      try
      {
        await using var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration.GetValue<string>("Neo4JSettings:Database")));
        var readResults = await session.ExecuteReadAsync(async tx =>
        {
          var result = await tx.RunAsync(query);
          return await result.ToListAsync();
        });

        return readResults;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async static Task<List<IRecord>> RunExecuteReadAsync(IDriver _driver, IConfiguration _configuration, string query, IDictionary<string, object> parameters)
    {
      try
      {
        await using var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration.GetValue<string>("Neo4JSettings:Database")));
        var readResults = await session.ExecuteReadAsync(async tx =>
        {
          var result = await tx.RunAsync(query, parameters);
          return await result.ToListAsync();
        });

        return readResults;
      }
      catch (Neo4jException ex)
      {
        throw new Exception(ex.Message);
      }
    }

  }
}
