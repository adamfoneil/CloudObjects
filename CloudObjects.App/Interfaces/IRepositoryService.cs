using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

namespace CloudObjects.App.Interfaces
{
    public interface IRepositoryService<TEntity, in TEntityKey>
        where TEntity : class, IEntity<TEntityKey>, new()
        where TEntityKey : struct
    {
        Task<TEntity> GetAsync(TEntityKey id);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<bool> ExistsAsync(TEntityKey id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> ReplaceAsync(TEntity entity);
        Task<TEntity> PatchAsync(TEntityKey id, JsonPatchDocument<TEntity> patchDocument);

        Task DeleteAsync(TEntityKey id);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
