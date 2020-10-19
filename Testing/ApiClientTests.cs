using CloudObjects.Client;
using CloudObjects.Client.Models;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Static;

namespace Testing
{
    /// <summary>
    /// before running, launch an instance of CloudObjects.App locally without debugger (Ctrl+F5)
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

        [TestMethod]
        public void CreateAccount()
        {
            var client = new CloudObjectsClient(Host.Local);
            var account = client.CreateAccountAsync(testAccount).Result;

            client = new CloudObjectsClient(Host.Local, new ApiCredentials(account.Name, account.Key));
            client.DeleteAccountAsync().Wait();
        }

        [TestMethod]
        public void CreateObject()
        {
            var client = new CloudObjectsClient(Host.Local);
            var account = client.CreateAccountAsync(testAccount).Result;
        }
    }
}
