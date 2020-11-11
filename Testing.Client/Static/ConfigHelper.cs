using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Testing.Static
{
    internal static class ConfigHelper
    {
        internal static SqlConnection GetConnection() => new SqlConnection(GetConfig().GetConnectionString("Default"));

        internal static IConfigurationRoot GetConfig() => new ConfigurationBuilder()
            .AddJsonFile("Config/connection.json")            
            .Build();
    }
}
