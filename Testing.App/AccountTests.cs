using CloudObjects.App;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace Testing.App
{
    /// <summary>
    /// help from https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps
    /// </summary>
    [TestClass]
    public class AccountTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public AccountTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [TestMethod]
        public void Post()
        {
            //var service = new AccountService()
        }
    }
}
