﻿using CloudObjects.Client.Interfaces;
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
    public class CloudObjectsAuthClient
    {
        private readonly ApiCredentials _credentials;
        private readonly ICloudObjectsAuth _api = null;
        private readonly HostLocations _location;

        private string _token = null;

        public CloudObjectsAuthClient(string accountName, string accountKey) : this(HostLocations.Online, new ApiCredentials(accountName, accountKey))
        {
        }

        public CloudObjectsAuthClient(HostLocations location, string accountName, string accountKey) : this(location, new ApiCredentials(accountName, accountKey))
        {
        }

        public CloudObjectsAuthClient(HostLocations location, ApiCredentials credentials = null)
        {
            _credentials = credentials;
            _location = location;
            _api = RestService.For<ICloudObjectsAuth>(Host.Urls[location], new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
            });
        }
        
        public ITokenSaver TokenSaver { get; set; }

        public async Task LoginAsync()
        {
            if (TokenSaver != null)
            {
                string savedToken = await TokenSaver.GetAsync(_credentials.AccountName);
                if (!string.IsNullOrEmpty(savedToken)) _token = savedToken;
            }
            
            if (IsLoggedIn()) return;

            var api = RestService.For<ICloudObjectsToken>(Host.Urls[_location]);
            _token = await api.GetTokenAsync(_credentials);            

            if (TokenSaver != null)
            {
                await TokenSaver?.SaveAsync(_credentials.AccountName, _token);
            }
        }

        public async Task LogoutAsync()
        {
            _token = null;
            await TokenSaver?.DeleteAsync(_credentials.AccountName);
        }

        private bool IsLoggedIn() => !string.IsNullOrEmpty(_token);

        public async Task<Account> CreateAccountAsync(string name) => await _api.CreateAccountAsync(name);

        public async Task<CloudObject<T>> CreateAsync<T>(string name, T @object)
        {
            await LoginAsync();

            var result = await _api.CreateAsync(new StoredObject()
            {
                Name = name,
                Json = JsonSerializer.Serialize(@object)
            });

            return new CloudObject<T>(@object, result);
        }

        public async Task<CloudObject<T>> SaveAsync<T>(string name, T @object)
        {
            await LoginAsync();

            var result = await _api.SaveAsync(new StoredObject()
            {
                Name = name,
                Json = JsonSerializer.Serialize(@object)
            });

            return new CloudObject<T>(@object, result);
        }

        public async Task<CloudObject<T>> GetAsync<T>(string name)
        {
            await LoginAsync();

            var result = await _api.GetAsync(name);

            return CloudObject<T>.FromStoredObject(result);
        }

        public async Task DeleteAsync(string name)
        {
            await LoginAsync();
            await _api.DeleteAsync(name);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            await LoginAsync();
            return await _api.ExistsAsync(name);
        }

        public async Task<IEnumerable<CloudObject<T>>> ListAsync<T>(ListObjectsQuery query)
        {
            await LoginAsync();
            var results = await _api.ListAsync(query);
            return results.Select(storedObj => CloudObject<T>.FromStoredObject(storedObj));
        }
    }
}