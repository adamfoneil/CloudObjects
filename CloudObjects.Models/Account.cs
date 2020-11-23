using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudObjects.App.Models;

namespace CloudObjects.Models
{
    [Table(nameof(Account))]
    public class Account : AuditedEntity<long>
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Key { get; set; }

        /// <summary>
        /// if we were going to make a monetized service, then I figured we would want 
        /// to track some kind of renewal date, but this is not enforced at all currently
        /// </summary>
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow.AddDays(30);

        public override long AuditAccountId => Id;

        public override long AuditObjectId => 0;
    }
}
