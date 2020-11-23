using System.Collections.Generic;
using System.Threading.Tasks;
using CloudObjects.App.Queries;
using CloudObjects.Models;

namespace CloudObjects.App.Interfaces
{
    public interface IStoredObjectService : IRepositoryService<StoredObject, long>
    {
        Task<StoredObject> GetAsync(long accountId, string name);
        Task<bool> ExistsAsync(long accountId, string name);
        Task<List<StoredObject>> ListAsync(long accountId, ListStoredObjects filter);

        Task<StoredObject> RenameAsync(long accountId, string name, string newName);
        Task DeleteAsync(long accountId, string name = null);
    }
}
