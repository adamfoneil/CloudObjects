using CloudObjects.Client;
using CloudObjects.Client.Models;
using CloudObjects.Client.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Testing.Client.Models;
using Testing.Client.Static;
using Testing.Static;

namespace Testing
{
    /// <summary>
    /// before running, launch an instance of CloudObjects.App locally without debugger (set as startup, then Ctrl+F5)
    /// </summary>
    [TestClass]
    public class ApiClientAuthTests
    {
        const string testAccount = "sample1238";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            DbUtil.DeleteAccount(testAccount, ConfigHelper.GetConnection);
        }

        private CloudObjectsAuthClient GetClient()
        {
            var account = DbUtil.GetTestAccountAsync(testAccount, ConfigHelper.GetConnection).Result;
            var client = new CloudObjectsAuthClient(HostLocations.Local, account.Name, account.Key);
            return client;
        }

        [TestMethod]
        public void CreateObject()
        {
            var client = GetClient();

            var obj = client.CreateAsync("object101", new SampleObject()
            {
                FirstName = "nobody",
                LastName = "anyone",
                Address = "343 Whatever St"
            }).Result;
        }

        [TestMethod]
        public void ListObjects()
        {
            var client = GetClient();

            for (int i = 0; i < 65; i++)
            {
                client.SaveAsync($"list/object{i}", new SampleObject()
                {
                    FirstName = $"first-name{i}",
                    LastName = $"last-name{i}"
                }).Wait();
            }

            var page1 = client.ListAsync<SampleObject>(new ListObjectsQuery()
            {
                NameStartsWith = "list/",
                Page = 0
            }).Result;

            Assert.IsTrue(page1.Count() == 50);

            var page2 = client.ListAsync<SampleObject>(new ListObjectsQuery()
            {
                NameStartsWith = "list/",
                Page = 1
            }).Result;

            Assert.IsTrue(page2.Count() == 15);
        }
    }
}
