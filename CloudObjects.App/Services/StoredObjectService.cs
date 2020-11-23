using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudObjects.App.Bases;
using CloudObjects.App.Data;
using CloudObjects.App.Interfaces;
using CloudObjects.App.Queries;
using CloudObjects.Enums;
using CloudObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudObjects.App.Services
{
    public class StoredObjectService : RepositoryServiceBase<StoredObject, long>, IStoredObjectService
    {
        public StoredObjectService(CloudObjectsDbContext dbContext)
            : base(dbContext)
        {
        }

        public Task<StoredObject> GetAsync(long accountId, string name)
            => GetAsync(e => e.AccountId == accountId && e.Name == name);

        public Task<bool> ExistsAsync(long accountId, string name)
            => ExistsAsync(e => e.AccountId == accountId && e.Name == name);

        public Task<List<StoredObject>> ListAsync(long accountId, ListStoredObjects filter)
        {
            var query = DbContext.StoredObjects.Where(e => e.AccountId == accountId);

            if (!string.IsNullOrEmpty(filter.NameContains))
            {
                query = query.Where(e => EF.Functions.Like(e.Name, $"%{filter.NameContains}%"));
            } 
            else if (!string.IsNullOrEmpty(filter.NameStartsWith))
            {
                query = query.Where(e => EF.Functions.Like(e.Name, $"{filter.NameStartsWith}%"));
            } 
            
            query = filter.Sort switch
            {
                ListStoredObjectsSortOptions.DateAscending => query
                    .OrderBy(e => e.DateModified)
                    .ThenBy(e => e.DateCreated),

                ListStoredObjectsSortOptions.DateDescending => query
                    .OrderByDescending(e => e.DateModified)
                    .ThenByDescending(e => e.DateCreated),

                _ => query.OrderBy(e => e.Name)
            };

            var page = (filter.Page ?? 1) - 1;
            return query
                .Skip(page * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<StoredObject> RenameAsync(long accountId, string name, string newName)
        {
            var entity = await GetAsync(accountId, name);
            entity.Name = newName;
            return await UpdateAsync(entity);
        }

        public Task DeleteAsync(long accountId, string name = null)
            => name == null
                ? DeleteAsync(e => e.AccountId == accountId)
                : DeleteAsync(e => e.AccountId == accountId && e.Name == name);
    }
}
