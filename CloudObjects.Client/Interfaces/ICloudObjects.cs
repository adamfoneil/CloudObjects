using CloudObjects.Client.Models;
using CloudObjects.Interfaces;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudObjects.Client.Interfaces
{
    [Headers("Authorization: Bearer")]
    internal interface ICloudObjects
    {
        /// <summary>
        /// needs to be a separate interface because we can't require a token before we have a token!
        /// </summary>
        [Post("/api/Account/Token")]
        Task<string> GetTokenAsync([Body] ApiCredentials login);

        [Post("/api/Account")]
        Task<Account> CreateAccountAsync(string name);

        [Put("/api/Account?newName={newName}")]
        Task<Account> RenameAccountAsync(string newName);

        [Post("/api/Objects/")]
        Task<StoredObject> CreateAsync([Body] StoredObject @object);

        [Put("/api/Objects/")]
        Task<StoredObject> SaveAsync([Body] StoredObject @object);

        [Get("/api/Objects/{name}")]
        Task<StoredObject> GetAsync(string name);

        [Get("/api/Objects/exists/{name}")]
        Task<bool> ExistsAsync(string name);

        [Delete("/api/Objects/{name}")]
        Task DeleteAsync(string name);

        [Get("/api/Objects/List")]
        Task<IEnumerable<StoredObject>> ListAsync([Body] IListObjectsQuery query);

        [Put("/api/Objects/Rename?oldName={oldName}&newName={newName}")]
        Task RenameAsync(string oldName, string newName);
    }
}
