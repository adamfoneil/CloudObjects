using CloudObjects.Client;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
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
            using (var cn = ConfigHelper.GetConnection())
            {
                cn.Execute(
                    @"DELETE [act] 
                    FROM [dbo].[Activity] [act]
                    INNER JOIN [dbo].[Account] [a] ON [act].[AccountId]=[a].[Id]
                    WHERE [a].[Name]=@testAccount", new { testAccount });

                cn.Execute(
                    @"DELETE [so]
                    FROM [dbo].[StoredObject] [so]
                    INNER JOIN [dbo].[Account] [a] ON [so].[AccountId]=[a].[Id]
                    WHERE [a].[Name]=@testAccount", new { testAccount });

                cn.Execute("DELETE [a] FROM [dbo].[Account] [a] WHERE [Name]=@testAccount", new { testAccount });
            }
        }

        private async Task<Account> GetTestAccountAsync()
        {
            using (var cn = ConfigHelper.GetConnection())
            {
                Account result = await cn.QuerySingleOrDefaultAsync<Account>("SELECT * FROM [dbo].[Account] WHERE [Name]=@testAccount", new { testAccount });

                if (result == null)
                {
                    var client = new CloudObjectsClient(Host.Local);
                    result = await client.CreateAccountAsync(testAccount);
                }

                return result;
            }            
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
            var account = GetTestAccountAsync().Result;
            var client = new CloudObjectsClient(Host.Local, account.Name, account.Key);
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

        public class SampleObject
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
        }
    }
}
