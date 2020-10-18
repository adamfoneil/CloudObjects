using AO.Models.Enums;
using AO.Models.Interfaces;
using Dapper.CX.SqlServer.Extensions.Long;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CloudObjects.Models.Conventions
{
    public abstract partial class AuditedTable : IAudit, ITrigger
    {
        public async Task RowDeletedAsync(IDbConnection connection, IDbTransaction txn = null, IUserBase user = null)
        {
            var activity = new Activity()
            {
                AccountId = AuditAccountId,
                ObjectId = AuditObjectId,
                Operation = Operation.Delete
            };

            await connection.SaveAsync(activity, txn: txn);
        }

        public async Task RowSavedAsync(IDbConnection connection, SaveAction saveAction, IDbTransaction txn = null, IUserBase user = null)
        {
            var activity = new Activity()
            {
                AccountId = AuditAccountId,
                ObjectId = AuditObjectId,
                Operation = (saveAction == SaveAction.Insert) ? Operation.Create : Operation.Update
            };

            await connection.SaveAsync(activity, txn: txn);
        }

        public async Task RowSavingAsync(IDbConnection connection, SaveAction saveAction, IDbTransaction txn = null, IUserBase user = null)
        {
            await Task.CompletedTask;
        }

        public void Stamp(SaveAction saveAction, IUserBase user)
        {
            if (saveAction == SaveAction.Update)
            {
                DateModified = DateTime.UtcNow;
            }
        }
    }
}
