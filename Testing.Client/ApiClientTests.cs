using CloudObjects.Client;
using CloudObjects.Client.Models;
using CloudObjects.Client.Static;
using CloudObjects.Models;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Testing.Client.Models;
using Testing.Client.Static;
using Testing.Static;

namespace Testing
{
    /// <summary>
    /// before running, launch an instance of CloudObjects.App locally without debugger (set as startup, then Ctrl+F5)
    /// </summary>
    [TestClass]
    public class ApiClientTests
    {
        const string testAccount = "sample1238";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            DbUtil.DeleteAccount(testAccount, ConfigHelper.GetConnection);
        }        

        [TestMethod]
        public void CreateAccount()
        {
            var client = GetClient();
            client.DeleteAccountAsync().Wait();
        }

        [TestMethod]
        public void CreateObject()
        {
            var client = GetClient();

            var obj = client.CreateAsync("object1", new SampleObject()
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
        public void SaveObject()
        {
            CloudObjectsClient client = GetClient();

            var sample = new SampleObject()
            {
                FirstName = "whoosie",
                LastName = "whatsie",
                Address = "887 yodalay"
            };

            var obj = client.SaveAsync("object3", sample).Result;

            Assert.IsTrue(obj.Id != 0);
            Assert.IsTrue(obj.Object.LastName.Equals("whatsie"));

            sample.FirstName = "yiminy";
            var updated = client.SaveAsync("object3", sample).Result;

            Assert.IsTrue(updated.Id == obj.Id);
            Assert.IsTrue(sample.FirstName.Equals(updated.Object.FirstName));
        }

        private CloudObjectsClient GetClient()
        {
            var account = DbUtil.GetTestAccountAsync(testAccount, ConfigHelper.GetConnection).Result;
            var client = new CloudObjectsClient(HostLocations.Local, account.Name, account.Key);
            return client;
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
        public void GetById()
        {
            var client = GetClient();

            var obj = client.SaveAsync("test/hello", new SampleObject()
            {
                FirstName = "yessee",
                LastName = "whoopsie"
            }).Result;

            var fetched = client.GetAsync<SampleObject>(obj.Id).Result;
            Assert.IsTrue(obj.Object.FirstName.Equals(fetched.Object.FirstName));
            Assert.IsTrue(obj.Object.LastName.Equals(fetched.Object.LastName));
            Assert.IsTrue(obj.Id == fetched.Id);
        }

        [TestMethod]
        public void GetByName()
        {
            var client = GetClient();

            var obj = client.SaveAsync("test/hello", new SampleObject()
            {
                FirstName = "yessee",
                LastName = "whoopsie"
            }).Result;

            var fetched = client.GetAsync<SampleObject>(obj.Name).Result;
            Assert.IsTrue(obj.Object.FirstName.Equals(fetched.Object.FirstName));
            Assert.IsTrue(obj.Object.LastName.Equals(fetched.Object.LastName));
            Assert.IsTrue(obj.Id == fetched.Id);
        }        
    }
}
