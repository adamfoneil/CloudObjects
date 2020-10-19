using CloudObjects.App.Queries;
using Dapper.QX;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Testing
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void ListStoredObjectsQuery() => QueryHelper.Test<ListStoredObjects>(GetConnection);

        private IDbConnection GetConnection() => new SqlConnection(GetConfig().GetConnectionString("Default"));

        private IConfigurationRoot GetConfig() => new ConfigurationBuilder()
            .AddJsonFile("Config/connection.json")
            .Build();
    }
}
