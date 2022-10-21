using API.Cryptography;
using API.Models.V1;
using Neo4j.Driver;

namespace API.Database
{
  public interface ILogRepository
  {
    Task<int?> CreateLog(ILogModel logToAdd);
    Task<ILogModel?> GetById(int id);
  }

  public class LogRepository : ILogRepository
  {
    private readonly IConfiguration _configuration;
    private bool _disposed = false;
    private readonly IDriver _driver;
    private readonly ICryptographyUtil _cryptoUtil;

    #region Database Actions
    public async Task<ILogModel?> GetById(int id)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteReadAsync(async transaction =>
        {
          var query = @"MATCH (log:Log) 
                     WHERE id(log) = $id
                     RETURN collect({
                              id: log.id,
                              data_avant: log.data_avant,
                              data_apres: log.data_apres
                            }) as log";

          var parameters = new Dictionary<string, object> {
            { "id", id }
          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => ToUserModel(record["log"].As<List<IDictionary<string, object>>>()));
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }

    public async Task<int?> CreateLog(ILogModel logToAdd)
    {
      var session = _driver.AsyncSession(configBuilder => configBuilder.WithDatabase(_configuration["Neo4JSettings:Database"]));
      try
      {
        return await session.ExecuteWriteAsync(async transaction =>
        {
          var query = "CREATE (log:Log {data_avant: $data_avant, data_apres: $data_apres}) RETURN log";

          Dictionary<string, object> parameters = new()
          {

          };

          var cursor = await transaction.RunAsync(query, parameters);

          return await cursor.SingleAsync(record => record["log"].As<INode>().ElementId.As<int>());
        });
      }
      finally
      {
        await session.CloseAsync();
      }
    }
    #endregion

    #region Cast
    private static LogModel? ToUserModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return ToListLogModel(datas).FirstOrDefault();
    }
    private static List<LogModel> ToListLogModel(IEnumerable<IDictionary<string, object>> datas)
    {
      return datas.Select(dictionary => new LogModel
      {
        Id = dictionary["id"].As<int>(),
        Data_after = dictionary["data_after"].As<string>(),
        Data_before = dictionary["data_before"].As<string>(),
      }).ToList();
    }
    #endregion

    #region Constructeur et Dispose
    public LogRepository(IConfiguration configuration, ICryptographyUtil cryptoUtil, IDriver driver)
    {
      _cryptoUtil = cryptoUtil;
      _configuration = configuration;
      _driver = driver;
    }

    ~LogRepository() => Dispose(false);
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