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
        [TestMethod]
        public void CreateAccount()
        {
            var client = new CloudObjectsClient(Host.Local);
            var account = client.CreateAccountAsync("sample1238").Result;

            client = new CloudObjectsClient(Host.Local, new ApiCredentials(account.Name, account.Key));
            client.DeleteAccountAsync().Wait();
        }

        [TestMethod]
        public void CreateObject()
        {

        }
    }
}
