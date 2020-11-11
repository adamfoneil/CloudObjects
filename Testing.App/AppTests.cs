using CloudObjects.App;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Testing.Client.Static;
using Testing.Static;

namespace Testing.App
{
    /// <summary>
    /// help from https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps
    /// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    [TestClass]
    public class AppTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        private const string _testAccountName = "sample-1239";

        public AppTests()
        {
            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                BaseAddress = new Uri("https://localhost:44328")
            });
        }

        [TestMethod]
        public void CreateAndDeleteAccount()
        {
            var account = GetTestAccount();
            Login(account);
            
            DeleteAccount();
        }

        private void DeleteAccount()
        {
            var result = _client.DeleteAsync("/api/Account").Result;
            Assert.IsTrue(result.IsSuccessStatusCode);            
        }

        [TestMethod]
        public void CreteAndDeleteObject()
        {
            var account = GetTestAccount();
            Login(account);

            const string data = "{ \"greeting\" : \"hello\" }";
            const string objName = "sample/hello";

            var result = _client.PostAsJsonAsync("/api/Objects", new StoredObject()
            {
                AccountId = account.Id,
                Name = objName,
                Json = data,
                Length = data.Length
            }).Result;

            var storedObj = result.Content.ReadFromJsonAsync<StoredObject>().Result;
            Assert.IsTrue(storedObj.AccountId == account.Id);
            Assert.IsTrue(storedObj.Name.Equals(objName));

            result = _client.DeleteAsync($"/api/Objects/{storedObj.Name}").Result;
            Assert.IsTrue(result.IsSuccessStatusCode);

            result = _client.DeleteAsync($"/api/Account").Result;
            Assert.IsTrue(result.IsSuccessStatusCode);
        }  
        
        private Account GetTestAccount()
        {
            DbUtil.DeleteAccount(_testAccountName);

            var result = _client.PostAsync($"/api/Account?name={_testAccountName}", null).Result;
            if (result.IsSuccessStatusCode)
            {
                return result.Content.ReadFromJsonAsync<Account>().Result;
            }

            var message = result.Content.ReadAsStringAsync().Result;
            throw new Exception($"Error creating test account: {message}");
        }

        private void Login(Account account)
        {
            var result = _client.PostAsJsonAsync("/api/Account/Token", new ApiCredentials()
            {
                AccountName = account.Name,
                AccountKey = account.Key
            }).Result;
            var token = result.Content.ReadAsStringAsync().Result;
            
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);            
        }                      
    }
}
