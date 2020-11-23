using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CloudObjects.App.Data;
using CloudObjects.App.Exception;
using CloudObjects.App.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace CloudObjects.App.Bases
{
    public abstract class RepositoryServiceBase<TEntity, TEntityKey> : IRepositoryService<TEntity, TEntityKey>
        where TEntity : class, IEntity<TEntityKey>, new()
        where TEntityKey : struct
    {
        protected CloudObjectsDbContext DbContext { get; }

        protected RepositoryServiceBase(CloudObjectsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<TEntity> GetAsync(TEntityKey id)
        {
            var entity = await DbContext.FindAsync<TEntity>(id);
            if (entity == null)
            {
                throw new EntityNotFoundException($"Entity of type {nameof(TEntity)} with ID {id} was not found");
            }

            return entity;
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await DbContext.Set<TEntity>().Where(predicate).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new EntityNotFoundException($"Entity of type {nameof(TEntity)} was not found by filter: {predicate}");
            }

            return entity;
        }

        public virtual async Task<bool> ExistsAsync(TEntityKey id)
        {
            var entity = await DbContext.FindAsync<TEntity>(id);
            return entity != null;
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
            => DbContext.Set<TEntity>().Where(predicate).AnyAsync();

        public virtual Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate)
            => DbContext.Set<TEntity>().Where(predicate).ToListAsync();

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            DbContext.Update(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<TEntity> ReplaceAsync(TEntity entity)
        {
            // Correct me if I wrongly understood the logic of Data.MergeAsync
            DbContext.Attach(entity);
            DbContext.Update(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<TEntity> PatchAsync(TEntityKey id, JsonPatchDocument<TEntity> patchDocument)
        {
            var entity = await GetAsync(id);

            patchDocument.ApplyTo(entity);
            return await UpdateAsync(entity);
        }

        public virtual async Task DeleteAsync(TEntityKey id)
        {
            var entity = await GetAsync(id);

            DbContext.Remove(entity);
            await DbContext.SaveChangesAsync();
        }


        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = DbContext.Set<TEntity>().Where(predicate);
            DbContext.RemoveRange(entities);

            await DbContext.SaveChangesAsync();
        }
    }
}
