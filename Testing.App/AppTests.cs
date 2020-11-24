using CloudObjects.App;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Testing.Client.Static;

namespace Testing.App
{
    /// <summary>
    /// help from https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps
    /// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    [TestClass]
    public class ControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        private const string sampleData = "{ \"greeting\" : \"hello\" }";
        private const string _testAccountName = "sample-1239";

        public ControllerTests()
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

        [TestMethod]
        public void DeleteAllObjects()
        {
            var account = GetTestAccount();
            Login(account);

            _client.PostAsJsonAsync("/api/Objects", new StoredObject()
            {
                Name = "sample.object",
                Json = sampleData
            });

            var result = _client.DeleteAsync("/api/Objects/All").Result;
            Assert.IsTrue(result.IsSuccessStatusCode);
            var countStr = result.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(Convert.ToInt32(countStr) > 0);
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

            

            // important: slashes don't work in names because there's no implicit URL 
            // encoding going on here that otherwise seems to be automatic in Refit
            const string objName = "sample.hello";

            var result = _client.PostAsJsonAsync("/api/Objects", new StoredObject()
            {                
                Name = objName,
                Json = sampleData
            }).Result;

            var storedObj = result.Content.ReadFromJsonAsync<StoredObject>().Result;
            Assert.IsTrue(storedObj.AccountId == account.Id);
            Assert.IsTrue(storedObj.Name.Equals(objName));

            result = _client.GetAsync($"/api/Objects/{storedObj.Name}").Result;
            Assert.IsTrue(result.IsSuccessStatusCode);

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
