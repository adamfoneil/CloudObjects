using CloudObjects.App;
using CloudObjects.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace Testing.App
{
    /// <summary>
    /// help from https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps
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

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(configure =>
                {
                    configure
                        .AddJsonFile("Config/appsettings.json")
                        .AddJsonFile("Config/connection.json")
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        [TestMethod]
        public void CreateAccount()
        {
            var result = _client.PostAsync("/api/Account?name=hello-account", null).Result;
            var account = result.Content.ReadFromJsonAsync<Account>().Result;
            Assert.IsTrue(account.Id != 0);
        }

    }
}
