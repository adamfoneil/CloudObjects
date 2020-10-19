using CloudObjects.Client;
using CloudObjects.Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
