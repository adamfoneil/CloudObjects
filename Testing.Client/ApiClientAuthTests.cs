using CloudObjects.Client;
using CloudObjects.Client.Models;
using CloudObjects.Client.Static;
using CloudObjects.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringIdLibrary;
using System;
using System.Linq;
using System.Text.Json;
using Testing.Client.Models;
using Testing.Static;

namespace Testing
{
    /// <summary>
    /// before testing locally, launch an instance of CloudObjects.App locally without debugger (set as startup, then Ctrl+F5).
    /// by default, tests will hit production URL
    /// </summary>
    [TestClass]
    public class ApiClientAuthTests
    {
        private static HostLocations _location;
        private static Account _testAccount;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {            
            _testAccount = GetTestAccount();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var client = GetClient();
            client.DeleteAllObjects().Wait();
            client.DeleteAccountAsync().Wait();
        }

        private static CloudObjectsClient GetClient()
        {            
            var client = new CloudObjectsClient(_location, _testAccount.Name, _testAccount.Key);

            // token saver doesn't work in these tests because we keep re-building the test account with a new Id, which breaks any saved token
            //client.TokenSaver = new DbTokenSaver(() => LocalDb.GetConnection("CloudObjects"));

            return client;
        }

        /// <summary>
        /// creates a sample account that will be deleted at the end of the tests
        /// </summary>        
        private static Account GetTestAccount()
        {
            var locationVal = ConfigHelper.GetConfig()["TestClientLocation"];
            _location = Enum.Parse<HostLocations>(locationVal);
            var client = new CloudObjectsClient(_location);
            var accountName = new StringIdBuilder().Add("test-").Add(8, StringIdRanges.Upper | StringIdRanges.Numeric).Build();
            return client.CreateAccountAsync(accountName).Result;
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
        public void GetCloudObjectByName()
        {
            var client = GetClient();

            var content = new SampleObject()
            {
                FirstName = "yessee",
                LastName = "whoopsie"
            };
            var obj = client.SaveAsync("test.hello", content).Result;

            var fetched = client.GetCloudObjectAsync<SampleObject>(obj.Name).Result;
            Assert.IsTrue(obj.Object.FirstName.Equals(fetched.Object.FirstName));
            Assert.IsTrue(obj.Object.LastName.Equals(fetched.Object.LastName));
            Assert.IsTrue(obj.Id == fetched.Id);
            Assert.IsTrue(obj.Length == JsonSerializer.Serialize(content).Length);
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
            var obj = client.SaveAsync("test.hello2", content).Result;

            var fetched = client.GetAsync<SampleObject>(obj.Name).Result;
            Assert.IsTrue(obj.Object.FirstName.Equals(fetched.FirstName));
            Assert.IsTrue(obj.Object.LastName.Equals(fetched.LastName));            
            Assert.IsTrue(obj.Length == JsonSerializer.Serialize(content).Length);
        }

        [TestMethod]
        public void RenameAccount()
        {
            var client = GetClient();

            client.RenameAccountAsync("the-new-name1").Wait();
            client.RenameAccountAsync(_testAccount.Name).Wait();
        }

        [TestMethod]
        public void RenameObject()
        {
            const string oldName = "test.whatever";
            const string newName = "test.very-whatever";

            var client = GetClient();
            var obj = client.SaveAsync(oldName, new SampleObject()
            {
                FirstName = "Oingo",
                LastName = "Boingo"
            }).Result;

            client.RenameObjectAsync(oldName, newName).Wait();

            Assert.IsTrue(client.ExistsAsync(newName).Result);
            Assert.IsTrue(!client.ExistsAsync(oldName).Result);
        }
    }
}
