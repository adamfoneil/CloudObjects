using CloudObjects.Client;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        private async Task<Account> InitTestAccountAsync()
        {
            var client = new CloudObjectsClient(Host.Local);
            var account = await client.CreateAccountAsync(testAccount);
            return account;
        }

        [TestMethod]
        public void CreateAccount()
        {
            var account = InitTestAccountAsync().Result;            

            var client = new CloudObjectsClient(Host.Local, new ApiCredentials(account.Name, account.Key));
            client.DeleteAccountAsync().Wait();
        }

        [TestMethod]
        public void CreateObject()
        {
            var account = InitTestAccountAsync().Result;
            var client = new CloudObjectsClient(Host.Local, account.Name, account.Key);

            var obj = client.CreateObjectAsync("object1", new SampleObject()
            {
                FirstName = "nobody",
                LastName = "anyone",
                Address = "343 Whatever St"
            }).Result;
        }

        public class SampleObject
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
        }
    }
}
