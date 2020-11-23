using System;
using CloudObjects.App.Interfaces;

namespace CloudObjects.App.Models
{
    public abstract class AuditedEntity<TEntityKey> : EntityBase<TEntityKey>, IAuditedEntity
        where TEntityKey : struct
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public abstract long AuditAccountId { get; }
        public abstract long AuditObjectId { get; }
    }
}
