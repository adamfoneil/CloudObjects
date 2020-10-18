using System;

namespace CloudObjects.Models.Conventions
{
    public abstract partial class AuditedTable : BaseTable
    {
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }

        public abstract long AuditAccountId { get; }
        public abstract long AuditObjectId { get; }
    }
}
