using CloudObjects.App.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Static;

namespace Testing.App
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void ListStoredObjectsQuery() => QueryHelper.Test<ListStoredObjects>(ConfigHelper.GetConnection);

        [TestMethod]
        public void DeleteAllObjectsQuery() => QueryHelper.Test<DeleteAllObjects>(ConfigHelper.GetConnection);
    }
}
