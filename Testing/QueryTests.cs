using CloudObjects.App.Queries;
using Dapper.QX;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Testing.Static;

namespace Testing
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void ListStoredObjectsQuery() => QueryHelper.Test<ListStoredObjects>(ConfigHelper.GetConnection);        
    }
}
