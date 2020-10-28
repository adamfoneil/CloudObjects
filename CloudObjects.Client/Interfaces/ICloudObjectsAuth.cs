using CloudObjects.Client.Models;
using CloudObjects.Interfaces;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudObjects.Client.Interfaces
{
    internal interface ICloudObjectsToken
    {
        /// <summary>
        /// needs to be a separate interface because we can't require a token before we have a token!
        /// </summary>
        [Post("/api/Account/Token")]
        Task<string> GetTokenAsync([Body] ApiCredentials login);
    }

    [Headers("Authorization: Bearer")]
    internal interface ICloudObjectsAuth
    {
        [Post("/api/Account")]
        Task<Account> CreateAccountAsync(string name);

        [Post("/api/Objects/")]
        Task<StoredObject> CreateAsync([Body] StoredObject @object);

        [Put("/api/Objects/")]
        Task<StoredObject> SaveAsync([Body] StoredObject @object);

        [Get("/api/Objects/{name}")]
        Task<StoredObject> GetAsync(string name);

        [Delete("/api/Objects/{name}")]
        Task DeleteAsync(string name);

        [Get("/api/Objects/List")]
        Task<IEnumerable<StoredObject>> ListAsync([Body] IListObjectsQuery query);
    }
}
