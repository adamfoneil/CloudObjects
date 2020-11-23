using System;

namespace CloudObjects.Models.Conventions
{
    public abstract class Audited : EntityBase
    {
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }

        public abstract long AuditAccountId { get; }
        public abstract long AuditObjectId { get; }
    }
}
