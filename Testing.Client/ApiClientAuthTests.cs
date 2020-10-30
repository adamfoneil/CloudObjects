using CloudObjects.Client;
using CloudObjects.Client.Models;
using CloudObjects.Client.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using System.Linq;
using System.Text.Json;
using Testing.Client;
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

        private CloudObjectsClient GetClient()
        {
            var account = DbUtil.GetTestAccountAsync(testAccount, ConfigHelper.GetConnection).Result;
            var client = new CloudObjectsClient(HostLocations.Local, account.Name, account.Key);
            //client.TokenSaver = new DbTokenSaver(() => LocalDb.GetConnection("CloudObjectsLocal"));
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
        public void UpdateObject()
        {
            var client = GetClient();

            var obj = client.CreateAsync("object2", new SampleObject()
            {
                FirstName = "nobody",
                LastName = "anyone",
                Address = "343 Whatever St"
            }).Result;

            obj.Object.FirstName = "anyone";
            var result = client.SaveAsync("object2", obj.Object).Result;
            Assert.IsTrue(result.Object.FirstName.Equals("anyone"));
        }

        [TestMethod]
        public void DeleteObject()
        {
            var client = GetClient();

            var obj = client.CreateAsync("object3", new SampleObject()
            {
                FirstName = "nobody",
                LastName = "anyone",
                Address = "343 Whatever St"
            }).Result;

            client.DeleteAsync("object3").Wait();

            Assert.IsTrue(!client.ExistsAsync("object3").Result);
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

        [TestMethod]
        public void GetByName()
        {
            var client = GetClient();

            var content = new SampleObject()
            {
                FirstName = "yessee",
                LastName = "whoopsie"
            };
            var obj = client.SaveAsync("test/hello", content).Result;

            var fetched = client.GetAsync<SampleObject>(obj.Name).Result;
            Assert.IsTrue(obj.Object.FirstName.Equals(fetched.Object.FirstName));
            Assert.IsTrue(obj.Object.LastName.Equals(fetched.Object.LastName));
            Assert.IsTrue(obj.Id == fetched.Id);
            Assert.IsTrue(obj.Length == JsonSerializer.Serialize(content).Length);
        }
    }
}
