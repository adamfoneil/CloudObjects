using CloudObjects.Client.Interfaces;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudObjects.Client
{
    public enum Host
    {
        Local,
        Online
    }

    public class CloudObjectsClient
    {
        private readonly ICloudObjects _api = null;
        private readonly ApiCredentials _credentials = null;

        private static Dictionary<Host, string> _urls = new Dictionary<Host, string>()
        {
            [Host.Local] = "https://localhost:44328",
            [Host.Online] = "https://cloudobjects.azurewebsites.net"
        };

        public CloudObjectsClient(string accountName, string accountKey) : this(Host.Online, new ApiCredentials(accountName, accountKey))
        {
        }

        public CloudObjectsClient(Host host, string accountName, string accountKey) : this(host, new ApiCredentials(accountName, accountKey))
        {
        }                

        public CloudObjectsClient(Host host, ApiCredentials credentials = null)
        {
            Host = host;
            _api = RestService.For<ICloudObjects>(_urls[Host]);
            _credentials = credentials;
        }

        public Host Host { get; }

        public async Task<Account> CreateAccountAsync(string name) => await _api.CreateAccountAsync(name);

        public async Task DeleteAccountAsync() => await _api.DeleteAccountAsync(_credentials);

        public async Task<StoredObject> CreateObjectAsync<T>(string name, T @object)
        {
            return await _api.CreateObjectAsync(_credentials, new StoredObject()
            {
                Name = name,
                Json = JsonSerializer.Serialize(@object)
            });
        }
        
    }
}
