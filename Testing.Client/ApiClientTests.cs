﻿using CloudObjects.Client;
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
            var account = GetTestAccountAsync().Result;            

            var client = new CloudObjectsClient(Host.Local, new ApiCredentials(account.Name, account.Key));
            client.DeleteAccountAsync().Wait();
        }

        [TestMethod]
        public void CreateObject()
        {
            var account = GetTestAccountAsync().Result;
            var client = new CloudObjectsClient(Host.Local, account.Name, account.Key);

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
            var account = GetTestAccountAsync().Result;
            var client = new CloudObjectsClient(Host.Local, account.Name, account.Key);

            var obj = client.CreateAsync("object2", new SampleObject()
            {
                FirstName = "nobody",
                LastName = "anyone",
                Address = "343 Whatever St"
            }).Result;

            obj.Object.FirstName = "anyone";
            var result = client.UpdateAsync(obj).Result;
            Assert.IsTrue(result.Object.FirstName.Equals("anyone"));
        }

        public class SampleObject
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
        }
    }
}