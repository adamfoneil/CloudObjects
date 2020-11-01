using CloudObjects.App;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
            var result = _client.PostAsync($"/api/Account?name=sample-1239", null).Result;
            Assert.IsTrue(result.IsSuccessStatusCode);

            var account = result.Content.ReadFromJsonAsync<Account>().Result;
            Assert.IsTrue(account.Id != 0);
            
            Login(account);

            result = _client.DeleteAsync("/api/Account").Result;
            Assert.IsTrue(result.IsSuccessStatusCode);
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
