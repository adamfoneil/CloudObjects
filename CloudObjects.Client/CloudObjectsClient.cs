using CloudObjects.Client.Interfaces;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudObjects.Client
{
    public enum HostLocation
    {
        Local,
        Online
    }

    public class CloudObjectsClient
    {
        private readonly ICloudObjects _api = null;
        private readonly ApiCredentials _credentials = null;

        private static Dictionary<HostLocation, string> _urls = new Dictionary<HostLocation, string>()
        {
            [HostLocation.Local] = "https://localhost:44328",
            [HostLocation.Online] = "https://cloudobjects.azurewebsites.net"
        };

        public CloudObjectsClient(HostLocation hostLocation, ApiCredentials credentials = null)
        {
            HostLocation = hostLocation;
            _api = RestService.For<ICloudObjects>(_urls[HostLocation]);
            _credentials = credentials;
        }

        public HostLocation HostLocation { get; }

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
