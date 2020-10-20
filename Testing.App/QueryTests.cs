using CloudObjects.App.Queries;
using Dapper.QX;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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