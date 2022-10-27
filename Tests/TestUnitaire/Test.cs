using Microsoft.Extensions.Configuration;

namespace Tests.TestUnitaire
{
  public class Test
  {
    public static IConfiguration InitConfiguration()
    {
      var config = new ConfigurationBuilder()
         .AddJsonFile("appsettings.test.json")
          .AddEnvironmentVariables()
          .Build();
      return config;
    }
  }
}