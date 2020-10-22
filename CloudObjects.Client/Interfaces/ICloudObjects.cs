using CloudObjects.Client.Models;
using CloudObjects.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudObjects.Client.Interfaces
{
    public interface ICloudObjects
    {        
        [Post("/api/Account")]
        Task<Account> CreateAccountAsync(string name);

        [Delete("/api/Account/{credentials.AccountName}?key={credentials.AccountKey}")]
        Task DeleteAccountAsync(ApiCredentials credentials);

        [Put("/api/Account/{credentials.AccountName}?key={credentials.AccountKey}")]
        Task<Account> UpdateAccountAsync(ApiCredentials credentials, [Body]Account account);

        /// <summary>
        /// inserts a new object, fails on duplicate name
        /// </summary>
        [Post("/api/Object/{credentials.AccountName}?key={credentials.AccountKey}")]
        Task<StoredObject> CreateObjectAsync(ApiCredentials credentials, [Body]StoredObject @object);

        /// <summary>
        /// inserts or updates an object, replacing if it already exists
        /// </summary>
        [Put("/api/Object/{credentials.AccountName}?key={credentials.AccountKey}")]
        Task<StoredObject> SaveObjectAsync(ApiCredentials credentials, [Body]StoredObject @object);

        [Post("/api/Object/{credentials.AccountName}/list?key={credentials.AccountKey}")]
        Task<IEnumerable<StoredObject>> ListObjectsAsync(ApiCredentials credentials, [Body]ListObjectsQuery query);

        [Get("/api/Object/{credentials.AccountName}/id/{id}?key={credentials.AccountKey}")]
        Task<StoredObject> GetByIdAsync(ApiCredentials credentials, long id);

        [Get("/api/Object/{credentials.AccountName}/name/{name}?key={credentials.AccountKey}")]
        Task<StoredObject> GetByNameAsync(ApiCredentials credentials, string name);
    }
}
