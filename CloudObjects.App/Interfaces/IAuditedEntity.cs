using System;

namespace CloudObjects.App.Interfaces
{
    public interface IAuditedEntity
    {
        DateTime DateCreated { get; set; }
        DateTime? DateModified { get; set; }

        long AuditAccountId { get; }
        long AuditObjectId { get; }
    }
}
