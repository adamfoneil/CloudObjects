using CloudObjects.Client.Models;
using CloudObjects.Models;
using Refit;
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

        [Post("/api/Object/{credentials.AccountName}?key={credentials.AccountKey}")]
        Task<StoredObject> CreateObjectAsync(ApiCredentials credentials, [Body]StoredObject @object);

        [Put("/api/Object/{credentials.AccountName}?key={credentials.AccountKey}")]
        Task<StoredObject> UpdateObjectAsync(ApiCredentials credentials, [Body]StoredObject @object);
    }
}
