using CloudObjects.Client.Interfaces;
using CloudObjects.Client.Models;
using CloudObjects.Client.Static;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudObjects.Client
{
    public class CloudObjectsClient
    {
        private readonly ICloudObjects _api = null;
        private readonly ApiCredentials _credentials = null;        

        public CloudObjectsClient(string accountName, string accountKey) : this(HostLocations.Online, new ApiCredentials(accountName, accountKey))
        {
        }

        public CloudObjectsClient(HostLocations location, string accountName, string accountKey) : this(location, new ApiCredentials(accountName, accountKey))
        {
        }                

        public CloudObjectsClient(HostLocations location, ApiCredentials credentials = null)
        {
            HostLocation = location;
            _api = RestService.For<ICloudObjects>(Host.Urls[location]);
            _credentials = credentials;
        }

        public HostLocations HostLocation { get; }

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
