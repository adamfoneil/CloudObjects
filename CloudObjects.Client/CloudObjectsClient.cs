using CloudObjects.Client.Interfaces;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<CloudObject<T>> CreateAsync<T>(string name, T @object)
        {
            var result = await _api.CreateObjectAsync(_credentials, new StoredObject()
            {
                Name = name,
                Json = JsonSerializer.Serialize(@object)
            });

            return new CloudObject<T>(@object, result);
        }

        public async Task<CloudObject<T>> SaveAsync<T>(string name, T @object)
        {
            var result = await _api.SaveObjectAsync(_credentials, new StoredObject()
            {
                Name = name,
                Json = JsonSerializer.Serialize(@object)
            });

            return new CloudObject<T>(@object, result);
        }

        public async Task<IEnumerable<CloudObject<T>>> ListAsync<T>(ListObjectsQuery query)
        {
            var results = await _api.ListObjectsAsync(_credentials, query);
            return results.Select(storedObj => CloudObject<T>.FromStoredObject(storedObj));
        }

        public async Task<CloudObject<T>> GetAsync<T>(long id)
        {
            var obj = await _api.GetByIdAsync(_credentials, id);
            return CloudObject<T>.FromStoredObject(obj);
        }

        public async Task<CloudObject<T>> GetAsync<T>(string name)
        {
            var obj = await _api.GetByNameAsync(_credentials, name);
            return CloudObject<T>.FromStoredObject(obj);
        }
    }
}
