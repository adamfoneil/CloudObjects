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

        public async Task<CloudObject<T>> CreateAsync<T>(string name, T @object)
        {
            var result = await _api.CreateObjectAsync(_credentials, new StoredObject()
            {
                Name = name,
                Json = JsonSerializer.Serialize(@object)
            });

            return GetCloudObject(@object, result);
        }

        public async Task<CloudObject<T>> UpdateAsync<T>(CloudObject<T> @object)
        {
            var result = await _api.UpdateObjectAsync(_credentials, new StoredObject()
            {
                Name = @object.Name,
                Json = JsonSerializer.Serialize(@object.Object)
            });

            return GetCloudObject(@object.Object, result);
        }

        private static CloudObject<T> GetCloudObject<T>(T @object, StoredObject result)
        {
            return new CloudObject<T>()
            {
                Object = @object,
                Name = result.Name,
                Id = result.Id,
                DateCreated = result.DateCreated,
                DateModified = result.DateModified,
                Length = result.Length,
            };
        }
    }
}
