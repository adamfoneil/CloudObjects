using System;
using System.Linq;
using System.Threading.Tasks;
using CloudObjects.App.Bases;
using CloudObjects.App.Data;
using CloudObjects.App.Interfaces;
using CloudObjects.Models;
using Microsoft.AspNetCore.JsonPatch;
using StringIdLibrary;

namespace CloudObjects.App.Services
{
    public class AccountService : RepositoryServiceBase<Account, long>, IAccountService
    {
        public AccountService(CloudObjectsDbContext dbContext)
            : base(dbContext)
        {
        }

        public Task<Account> CreateAsync(string name)
            => CreateAsync(new Account()
            {
                Name = name,
                Key = GetKey(),
                InvoiceDate = DateTime.UtcNow.AddDays(30)
            });

        public async Task<Account> RenameAsync(long id, string newName)
        {
            var account = await GetAsync(id);

            account.Name = newName;
            DbContext.Update(account);
            await DbContext.SaveChangesAsync();

            return account;
        }

        // Just do not want to apply the patch to the entire model, want to control the list of fields to be updated
        public override async Task<Account> PatchAsync(long id, JsonPatchDocument<Account> patchDocument)
        {
            var entity = await GetAsync(id);

            var patchEntity = new Account();
            patchDocument.ApplyTo(patchEntity);

            entity.Name = patchEntity.Name;

            return await UpdateAsync(entity);
        }

        public override async Task DeleteAsync(long id)
        {
            var account = await GetAsync(id);

            await using var transaction = await DbContext.Database.BeginTransactionAsync();
            try
            {
                var accountActivity = DbContext.Activity.Where(e => e.AccountId == account.Id);
                DbContext.RemoveRange(accountActivity);

                // Also in order to delete all account activity it's possible to execute an SQL command:
                // await DbContext.Database.ExecuteSqlInterpolatedAsync($"DELETE [dbo].[Activity] WHERE [AccountId]={account.Id}");

                DbContext.Remove(account);
                await DbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }
    }
}
