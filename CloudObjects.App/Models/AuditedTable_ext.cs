using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CloudObjects.Models
{
    public abstract class AuditedTableEx : IAudit, ITrigger
    {
        [JsonIgnore]
        public abstract bool TrackDeletions { get; }
        
        [NotMapped]
        public string Url { get; set; }

        public async Task RowDeletedAsync(IDbConnection connection, IDbTransaction txn = null, IUserBase user = null)
        {
            if (!TrackDeletions) return;

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
    }
}
